// Copyright (c) Anthony Ransley. All Rights Reserved.
// Licensed under GNU GPLv3, https://choosealicense.com/licenses/gpl-3.0/
// See the License file in the project root for license information.
// https://github.com/Software101DotNet

using AnalyseAUS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void SizeOnDisk()
		{
			var alloactionUnitSizes = new long[] { 4096, 8192, 16384, 65536 };
			foreach (var aus in alloactionUnitSizes)
			{
				var boundaryValues = new (long fileSize, long expectedSizeOnDisk)[]
				{
					(0, aus), (1, aus),
					(aus-1, aus), (aus, aus), (aus+1, 2*aus),
					(2*aus-1, 2*aus ), (2*aus , 2*aus), (2*aus+1, 3*aus)
				};

				foreach (var (fileSize, expectedSizeOnDisk) in boundaryValues)
				{
					var testsubject = new FileStatistic(fileSize);
					var sizeOnDisk = testsubject.SizeOnDisk(aus);
					Assert.AreEqual(expectedSizeOnDisk, sizeOnDisk);
				}
			}
		}
	}
}
