using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace Sourcery.IO.ZipFileSystem
{
    public class ZipFileRootDirectory : IRootDirectory
    {
        private readonly ZipFileSystemFs _fs;
        private readonly string _path;

        public ZipFileRootDirectory(ZipFileSystemFs fs, string path)
        {
            _fs = fs;
            _path = path;
            Directory.CreateDirectory(path);
        }


        public IEnumerable<IDirectory> EnumerateDirectories(string pattern)
        {
            return new DirectoryInfo(_path).EnumerateFiles(pattern).Select(fi => new ZipFileSystemDirectoryInfo(_fs, this, fi.FullName));
        }

        internal string FileName
        {
            get { return _path + ".zip"; }
        }
 
        public Fs Fs
        {
            get { return _fs; }
        }
 

        public IDirectory GetDirectoryInfo(string key)
        {
            return new ZipFileSystemDirectoryInfo(_fs, this, Path.Combine(_path, key + ".zip"));
        }
    }
}