using System;
using System.IO;
using System.Text;

namespace Sourcery.IO.FileSystem
{
    public class FileSystemFs : Sourcery.IO.Fs
    {
        private readonly string _rootPath;

        public FileSystemFs(string rootPath)
        {
            _rootPath = rootPath;
            Directory.CreateDirectory(_rootPath);
        }
 

        public override IRootDirectory Root
        {
            get
            {
                return new FileSystemDirectoryInfo(this, _rootPath);
            }
        }
    }
}
