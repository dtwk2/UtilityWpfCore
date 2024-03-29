using System;
using System.IO;
using Utility.FileSystem.Common;

namespace Utility.FileSystem.Transfer.Abstract.Infrastructure {
   public static class Helpers {
      // 1 KB = 1024 bytes
      private static readonly string[] SizeWindowsSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

      // 1 KiB = 1024 bytes
      private static readonly string[] SizeBinarySuffixes = { "bytes", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

      // 1 kB = 1000 bytes
      private static readonly string[] SizeMetricSuffixes = { "bytes", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

      internal static string ToSizeWithSuffix(long value, SuffixStyle style, int decimalPlaces = 1) {
         var newBase = 1024;
         if (style == SuffixStyle.Metric) {
            newBase = 1000;
         }

         if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
         if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

         // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
         int mag = (int)Math.Log(value, newBase);

         // 1L << (mag * 10) == 2 ^ (10 * mag) 
         // [i.e. the number of bytes in the unit corresponding to mag]
         decimal adjustedSize = (decimal)value / (1L << (mag * 10));

         if (style == SuffixStyle.Metric) {
            adjustedSize = value / (decimal)(Math.Pow(newBase, mag));
         }

         // make adjustment when the value is large enough that
         // it would round up to higher magnitude
         if (Math.Round(adjustedSize, decimalPlaces) >= 1000) {
            mag += 1;
            adjustedSize /= newBase;
         }

         return $"{adjustedSize:n2} {GetSuffixAtIndex(style, mag)}";
      }

      private static string GetSuffixAtIndex(SuffixStyle style, int index) {
         switch (style) {
            case SuffixStyle.Binary:
               return SizeBinarySuffixes[index];
            case SuffixStyle.Metric:
               return SizeMetricSuffixes[index];
            case SuffixStyle.Windows:
               return SizeWindowsSuffixes[index];
         }
         return string.Empty;
      }

      public static DirectorySizeInfo Size(this DirectoryInfo d) {
         DirectorySizeInfo size = new DirectorySizeInfo();

         try {
            // Add file sizes.
            var fis = d.GetFiles();
            foreach (var fi in fis) {
               size.Size += fi.Length;
            }
            size.FileCount += fis.Length;

            // Add subdirectory sizes.
            var dis = d.GetDirectories();
            size.DirectoryCount += dis.Length;
            foreach (var di in dis) {
               size += Size(di);
            }
         }
         catch (Exception ex) {
         }

         return size;
      }

      public sealed class DirectorySizeInfo {
         public long FileCount = 0;
         public long DirectoryCount = 0;
         public long Size = 0;

         public static DirectorySizeInfo operator +(DirectorySizeInfo s1, DirectorySizeInfo s2) {
            return new DirectorySizeInfo() {
               DirectoryCount = s1.DirectoryCount + s2.DirectoryCount,
               FileCount = s1.FileCount + s2.FileCount,
               Size = s1.Size + s2.Size
            };
         }
      }
   }
}
