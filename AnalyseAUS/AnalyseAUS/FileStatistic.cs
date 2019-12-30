// Copyright (c) Anthony Ransley. All Rights Reserved.
// Licensed under GNU GPLv3, https://choosealicense.com/licenses/gpl-3.0/
// See the License file in the project root for license information.
// https://github.com/Software101DotNet

using System;

namespace AnalyseAUS
{
	public class FileStatistic : IFileStatistic
	{
		public long Size { get; private set; }

		// calculates the number of bytes that would be allocated on disk for a given Allocation Unit Size.
		public long SizeOnDisk(long allocationUnitSize)
		{
			// note, even a file of 0 bytes is allocated with 1 allocation unit of disk.
			var allocationUnitsRequired = AllocationUnitCount(allocationUnitSize);
			return allocationUnitsRequired * allocationUnitSize;
		}

		public long AllocationUnitCount(long allocationUnitSize)
		{
			return (Size <= 0) ? 1 : (Size + (allocationUnitSize - 1)) / allocationUnitSize;
		}

		public FileStatistic(long size)
		{
			if (size < 0)
				throw new ArgumentOutOfRangeException(nameof(size), "file size can not be less than zero bytes.");

			Size = size;
		}
	}
}
