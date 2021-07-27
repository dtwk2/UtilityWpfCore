using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UtilityWpf.Utility
{
    public static class PathHelper
    {
        public static string[] GetFilesInSubDirectories(IList<FileSystemInfo> value)
            => value?
            .SelectMany(_ => Directory.GetFiles(_.FullName, "*.*", SearchOption.AllDirectories))
            .ToArray();

        public static string FileMap(string path)
            => DateTime
            .FromFileTime(Convert.ToInt64(Path.GetFileName(path).Replace(Path.GetExtension(path), "")))
            .ToShortDateString();
    }
}