// Copyright (c) Anthony Ransley. All Rights Reserved.
// Licensed under GNU GPLv3, https://choosealicense.com/licenses/gpl-3.0/
// See the License file in the project root for license information.
// https://github.com/Software101DotNet

using System;
using System.Collections.Generic;
using System.Linq;

namespace AnalyseAUS
{
	using static Extentions;

	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 1)
			{
				Process(args[0]);
			}
			else
			{
				Console.WriteLine();
				Console.WriteLine($"Copyright (c) Anthony Ransley. All Rights Reserved.");
				Console.WriteLine($"Licensed under the Apache License, Version 2.0.");
				Console.WriteLine($"See License.txt in the project root for license information");
				Console.WriteLine($"https://github.com/Software101DotNet");
				Console.WriteLine();
				Console.WriteLine($"Usage examples:");
				Console.WriteLine($"\tdotnet FileAllocationConsole.dll c:\\");
				Console.WriteLine($"\tdotnet FileAllocationConsole.dll d:\\photos");
			}
		}

		private static void Process(string root)
		{
			IScanFiles fileStats = new ScanFiles();

			Console.WriteLine($"Scanning {root}, please wait...");
			try
			{
				fileStats.ScanFileSystem(root);

				Console.WriteLine("Scan completed in {0}", fileStats.TimeTakenToComplete);
				ReportErrors(fileStats.ErrorReports);

				if (fileStats.FileStatistics.Count > 0)
				{
					Console.WriteLine();
					DisplayOverprovisioning(fileStats);

					var h = fileStats.HistogramFileSizes(fileStats.FileStatistics);
					DisplayHistogramStatistics(h);
				}
				else
				{
					Console.WriteLine($"Analysing and forecasting the file data size on disk for the possible allocation unit sizes supported by the Volume is not meaningful becuase no files where counted in {root}.");
				}
			}
			catch (System.IO.DirectoryNotFoundException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private static void DisplayOverprovisioning(IScanFiles fileStats)
		{
			var totalFileSize = fileStats.FileStatistics.Sum(s => s.Size);
			Console.WriteLine($"{fileStats.FileStatistics.Count:N0} files, totalling {totalFileSize.Decorated()}");
			Console.WriteLine($"These files will have the following size on disk depending on the Volume's formated Allocation Unit Size:");
			Console.WriteLine();

			var provisioningForcast = fileStats.CalcProvisioning(fileStats.FileStatistics);

			//Console.WriteLine($"   -----------------------------------------------------------------------------------");
			Console.WriteLine($"{"Formated allocation unit size",32} | {"Total size on disk",12} (includes overprovisioned space)");
			Console.WriteLine($"   -----------------------------------------------------------------------------------");

			for (var i = 0; i < provisioningForcast.Length; i++)
			{
				var overprovisioningPercentage = (100.0 * provisioningForcast[i].Overprovisioning) / provisioningForcast[i].SizeOnDisk;
				Console.WriteLine($"{provisioningForcast[i].AllocationUnitSize.Decorated(),32} | {provisioningForcast[i].SizeOnDisk.Decorated(),10} ({provisioningForcast[i].Overprovisioning.Decorated()} {overprovisioningPercentage,2:N1}%)");
			}
			//Console.WriteLine($"   -----------------------------------------------------------------------------------");
			Console.WriteLine();
		}

		private static void DisplayHistogramStatistics((long AllocationSize, long Frequency)[] histogram)
		{
			Console.WriteLine("Histogram of file sizes grouped by file allocation unit sizes:");
			Console.WriteLine();
			//Console.WriteLine($"   -----------------------------------------------------------------------------------");
			Console.WriteLine($"   {"file size",17} | {"file count",10} | {"%",8}");
			Console.WriteLine($"   -----------------------------------------------------------------------------------");

			long total = 0;
			foreach (var bin in histogram)
			{
				total += bin.Frequency;
			}

			for (var i = 0; i < histogram.Length; i++)
			{
				var datum = histogram[i].Frequency;
				var percentage = (100.0 * datum) / total;
				var range = i == 0 ?
					$"{histogram[i].AllocationSize.Decorated()}" :
					$"{histogram[i - 1].AllocationSize.Decorated()} - {histogram[i].AllocationSize.Decorated()}";

				Console.WriteLine($"   {range,17} | {datum,10} | {percentage,8:N2} {Convert.ToInt64(percentage).Graph()}");
			}

			Console.WriteLine();
		}

		private static void ReportErrors(IReadOnlyCollection<string> errorReports)
		{
			if (errorReports.Count > 0)
			{
				var baseColour = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"{errorReports.Count} Errors reported");
				Console.ForegroundColor = ConsoleColor.DarkRed;
				foreach (var e in errorReports)
				{
					Console.WriteLine(e);
				}
				Console.ForegroundColor = baseColour; // restore console foreground colour.
			}
		}
	}
}
