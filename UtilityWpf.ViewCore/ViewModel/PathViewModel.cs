using System;
using System.Collections.Generic;
using System.IO;

namespace UtilityWpf.ViewModel
{
    public class PathViewModel : FileSystemInfo
    {
        private Func<string, string> map;

        public string FilePath { get { return base.FullPath; } }

        public string Directory { get { return System.IO.Path.GetDirectoryName(FilePath); } }

        public override string Name
        {
            get
            {
                var name = map(FilePath);
                return String.IsNullOrEmpty(name) ? this.FilePath : name;
            }
        }

        public PathViewModel(string path, Func<string, string> map)
        {
            base.FullPath = path;
            this.map = map;
        }

        public string DisplayName { get; set; }

        public override bool Exists => true;

        public override string ToString() => FilePath;

        public override void Delete()
        {
            throw new NotImplementedException();
        }
    }

    public class FileViewModel : PathViewModel
    {
        public FileViewModel(string path, Func<string, string> map = null) : base(path, map ?? ((a) => System.IO.Path.GetFileName(a)))
        {
        }
    }

    public class DirectoryViewModel : PathViewModel
    {
        public DirectoryViewModel(string path, Func<string, string> map = null) : base(path, map ?? ((a) => System.IO.Path.GetDirectoryName(a)))
        {
        }

        private ICollection<PathViewModel> subDirectories;

        public ICollection<PathViewModel> SubDirectories
        {
            get
            {
                if (subDirectories == null)
                {
                    subDirectories = new List<PathViewModel>();
                    try
                    {
                        foreach (var dir in System.IO.Directory.GetDirectories(FilePath))
                        {
                            // get the file attributes for file or directory
                            FileAttributes attr = File.GetAttributes(dir);

                            //detect whether its a directory or file
                            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                                subDirectories.Add(new DirectoryViewModel(dir));
                            //else
                            //    subDirectories.Add(new FileViewModel(dir));
                        }
                    }
                    catch
                    {
                    }
                }
                return subDirectories;
            }
        }
    }
}