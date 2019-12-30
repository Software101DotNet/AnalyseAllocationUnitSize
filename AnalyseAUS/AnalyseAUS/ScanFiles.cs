// Copyright (c) Anthony Ransley. All Rights Reserved.
// Licensed under GNU GPLv3, https://choosealicense.com/licenses/gpl-3.0/
// See the License file in the project root for license information.
// https://github.com/Software101DotNet

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace AnalyseAUS
{
	public class ScanFiles : IScanFiles
	{
		public TimeSpan TimeTakenToComplete { get; private set; }
		public List<string> ErrorReports { get; } = new List<string>();
		public List<FileStatistic> FileStatistics { get; } = new List<FileStatistic>();

		public void ScanFileSystem(string rootFolder)
		{
			var startTime = DateTime.Now;

			var dir = new DirectoryInfo(rootFolder);
			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException($"The root folder {rootFolder} does not exist.");
			}

			ListFilesAndSubFolders(dir);

			TimeTakenToComplete = DateTime.Now - startTime;
		}

		private void ListFilesAndSubFolders(DirectoryInfo di)
		{
			if (di == null)
				throw new ArgumentNullException(nameof(di));

			try
			{
				var fsInfo = di.GetFileSystemInfos();

				// Iterate through each item. 
				foreach (var fsi in fsInfo)
				{
#if SpecialFileHandling
					// skip files that have Hidden, System or Temporary file attributes
					if (((fsi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) ||
						((fsi.Attributes & FileAttributes.System) == FileAttributes.System) ||
						((fsi.Attributes & FileAttributes.Temporary) == FileAttributes.Temporary))
						continue;
#endif
					switch (fsi)
					{
						case DirectoryInfo directoryInfo:
							ProcessDirectory(directoryInfo);
							break;

						case FileInfo fileInfo:
							ProcessFile(fileInfo);
							break;
					}
				}
			}
			catch (PathTooLongException e)
			{
				ErrorReports.Add(e.Message);
			}
			catch (ArgumentException e)
			{
				var msg = string.Concat(di.FullName, "\n", e.Message);
				ErrorReports.Add(msg);
			}
			catch (DirectoryNotFoundException e)
			{
				ErrorReports.Add(e.Message);
			}
			catch (SecurityException e)
			{
				ErrorReports.Add(e.Message);
			}
			catch (UnauthorizedAccessException e)
			{
				ErrorReports.Add(e.Message);
			}
		}

		private void ProcessDirectory(DirectoryInfo directoryInfo)
		{
			if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));

			ListFilesAndSubFolders(directoryInfo);
		}

		private void ProcessFile(FileInfo fileInfo)
		{
			if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

			FileStatistic fileStatistic = new FileStatistic(fileInfo.Length);
			FileStatistics.Add(fileStatistic);
		}

		public (long AllocationSize, long Frequency)[] HistogramFileSizes(IEnumerable<FileStatistic> fileSizes)
		{
			var histogram = new (long AllocationSize, long Frequency)[]
			{
				( 0x0000000000000000L,0),
				( 0x0000000000001000L,0),
				( 0x0000000000002000L,0 ),
				( 0x0000000000004000L,0 ),
				( 0x0000000000008000L,0 ),
				( 0x0000000000010000L,0 ),
				( 0x0000000000020000L,0 ),
				( 0x0000000000040000L,0 ),
				( 0x0000000000080000L,0 ),
				( 0x0000000000100000L,0 ),
				( long.MaxValue,0 )
			};

			long largestFileSize = 0;

			foreach (var fs in fileSizes)
			{
				if (fs.Size > largestFileSize)
					largestFileSize = fs.Size;

				for (var idx = 0; idx < histogram.Length; idx++)
				{
					if (fs.Size <= histogram[idx].AllocationSize)
					{
						histogram[idx].Frequency++;
						break;
					}
				}
			}

			if (largestFileSize > histogram[histogram.Length - 2].AllocationSize)
				histogram[histogram.Length - 1].AllocationSize = largestFileSize;

			return histogram;
		}

		// For each of the possible allocation unit sizes, calculate the sum total of the overproisioned space that would result.
		public (long AllocationUnitSize, long SizeOnDisk, long Overprovisioning)[] CalcProvisioning(IReadOnlyCollection<FileStatistic> fileStats)
		{
			var provisioningForcast = new (long AllocationUnitSize, long SizeOnDisk, long Overprovisioning)[]
			{
				( 0x0000000000001000L,0,0 ),
				( 0x0000000000002000L,0,0 ),
				( 0x0000000000004000L,0,0 ),
				( 0x0000000000008000L,0,0 ),
				( 0x0000000000010000L,0,0 ),
				( 0x0000000000020000L,0,0 ),
				( 0x0000000000040000L,0,0 ),
				( 0x0000000000080000L,0,0 ),
				( 0x0000000000100000L,0,0 )
			};

			for (var idx = 0; idx < provisioningForcast.Length; idx++)
			{
				foreach (var fs in fileStats)
				{
					var sizeOnDisk = fs.SizeOnDisk(provisioningForcast[idx].AllocationUnitSize);
					provisioningForcast[idx].SizeOnDisk += sizeOnDisk;
					provisioningForcast[idx].Overprovisioning += (sizeOnDisk - fs.Size);
				}
			}

			return provisioningForcast;
		}
	}
}
