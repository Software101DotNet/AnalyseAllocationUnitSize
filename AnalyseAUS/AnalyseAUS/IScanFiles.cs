// Copyright (c) Anthony Ransley. All Rights Reserved.
// Licensed under GNU GPLv3, https://choosealicense.com/licenses/gpl-3.0/
// See the License file in the project root for license information.
// https://github.com/Software101DotNet

using System;
using System.Collections.Generic;

namespace AnalyseAUS
{
	public interface IScanFiles
	{
		void ScanFileSystem(string rootFolder);
		List<FileStatistic> FileStatistics { get; }
		List<string> ErrorReports { get; }
		TimeSpan TimeTakenToComplete { get; }

		(long AllocationSize, long Frequency)[] HistogramFileSizes(IEnumerable<FileStatistic> fileSizes);
		(long AllocationUnitSize, long SizeOnDisk, long Overprovisioning)[] CalcProvisioning(IReadOnlyCollection<FileStatistic> fileStats);
	}
}