using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace Sourcery.IO.ZipFileSystem
{
    public class ZipFileRootDirectory : IRootDirectory
    {
        private readonly string _path;

        public ZipFileRootDirectory(string path)
        {
            _path = path;
            Directory.CreateDirectory(path);
        }


        public IEnumerable<IDirectory> EnumerateDirectories(string pattern)
        {
            return new DirectoryInfo(_path).EnumerateFiles(pattern).Select(fi => new ZipFileSystemDirectoryInfo(fi.FullName));
        }

        internal string FileName
        {
            get { return _path + ".zip"; }
        }
 

        public IDirectory GetDirectoryInfo(string key)
        {
            return new ZipFileSystemDirectoryInfo(Path.Combine(_path, key + ".zip"));
        }
    }
}