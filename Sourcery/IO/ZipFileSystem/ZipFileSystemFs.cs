using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sourcery.IO.ZipFileSystem
{
    public class ZipFileSystemFs : Sourcery.IO.Fs
    {
        private readonly string _rootPath;

        public ZipFileSystemFs(string rootPath)
        {
            _rootPath = rootPath;
            Directory.CreateDirectory(_rootPath);
        }
 

        public override IRootDirectory Root
        {
            get { return new ZipFileRootDirectory(this, _rootPath); }
        }
    }
}
