// Copyright (c) Anthony Ransley. All Rights Reserved.
// Licensed under GNU GPLv3, https://choosealicense.com/licenses/gpl-3.0/
// See the License file in the project root for license information.
// https://github.com/Software101DotNet

namespace AnalyseAUS
{
	public interface IFileStatistic
	{
		long Size { get; }

		long SizeOnDisk(long allocationUnitSize);
		long AllocationUnitCount(long allocationUnitSize);
	}
}