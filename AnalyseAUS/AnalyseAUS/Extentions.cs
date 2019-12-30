// Copyright (c) Anthony Ransley. All Rights Reserved.
// Licensed under GNU GPLv3, https://choosealicense.com/licenses/gpl-3.0/
// See the License file in the project root for license information.
// https://github.com/Software101DotNet

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseAUS
{
	public static class Extentions
	{
		public static int WordCount(this string str)
		{
			return str.Split(new char[] { ' ', '.', '?' },
				StringSplitOptions.RemoveEmptyEntries).Length;
		}

		// Value	IEC (Binary)	JEDEC (Binary)
		// 1024^1	KiB kibibyte	KB kilobyte
		// 1024^2	MiB mebibyte	MB megabyte
		// 1024^3	GiB gibibyte	GB gigabyte
		// 1024^4	TiB tebibyte	TB Terabyte
		// 1024^5	PiB pebibyte	PB Petabyte
		// 1024^6	EiB exbibyte	EB Exabyte
		// 1024^7	ZiB zebibyte	ZB Zettabyte
		// 1024^8	YiB yobibyte	YB Yottabyte

		public static string FormatedByteValues(this System.Int64 bytes)
		{
			var sb = new StringBuilder(1024);

			if (bytes < 1024)
			{
				sb.AppendFormat($"{bytes:N0} bytes");
			}
			else
			{
				double n = bytes;
				n /= 1024;
				if (n < (1024))
				{
					sb.AppendFormat($"{bytes:N} = {n:N} Kilobytes");
				}
				else
				{
					n /= 1024;
					if (n < (1024))
					{
						sb.AppendFormat($"{bytes:N} = {n:N} Megabytes");
					}
					else
					{
						n /= 1024;
						if (n < (1024))
						{
							sb.AppendFormat($"{bytes:N} = {n:N} Gigabytes");
						}
						else
						{
							n /= 1024;
							sb.AppendFormat($"{bytes:N} = {n} Terabytes");
						}
					}
				}
			}

			return sb.ToString();
		}

		public static string Graph(this long percentage)
		{
			if (percentage < 0 || percentage > 100) throw new ArgumentOutOfRangeException(nameof(percentage));

			var sb = new StringBuilder(1024);
			for (var i = 1; i <= percentage; i++)
			{
				sb.Append($"*");
			}

			return sb.ToString();
		}

		// For a given value of type long, the method returns a string of the value 
		// decorated with the unit group that the value falls within. Note, the value is 
		// effectively rounded down to the unit of the group. Example: 1090 would return 1KB
		public static string Decorated(this long bytes)
		{
			var sb = new StringBuilder(1024);
			long unit = 1024L;
			long extendedUnit = 100L;
			if (bytes < unit)
			{
				sb.AppendFormat($"{bytes:N0} B");
			}
			else
			{
				bytes /= 1024L;
				if (bytes < unit)
				{
					sb.AppendFormat($"{bytes:N0} KB");
				}
				else
				{
					bytes /= 1024L;
					if (bytes < unit)
					{
						sb.AppendFormat($"{bytes:N0} MB");
					}
					else
					{
						bytes /= 1024L;
						if (bytes < (unit* extendedUnit))
						{
							sb.AppendFormat($"{bytes:N0} GB");
						}
						else
						{
							bytes /= 1024L;
							if (bytes < (unit * extendedUnit))
							{
								sb.AppendFormat($"{bytes:N0} TB");
							}
							else
							{
								extendedUnit *= 10; // larger values benefit from even more precision
								bytes /= 1024L;
								if (bytes < (unit * extendedUnit))
								{
									sb.AppendFormat($"{bytes:N0} PB");
								}
								else
								{
									bytes /= 1024L;
									if (bytes < (unit * extendedUnit))
									{
										sb.AppendFormat($"{bytes:N0} EB");
									}
									else
									{
										bytes /= 1024L;
										if (bytes < (unit * extendedUnit))
										{
											sb.AppendFormat($"{bytes:N0} ZB");
										}
										else
										{
											bytes /= 1024L;
											sb.AppendFormat($"{bytes:N0} YB");
										}
									}
								}
							}
						}
					}
				}
			}

			return sb.ToString();
		}
	}
}

