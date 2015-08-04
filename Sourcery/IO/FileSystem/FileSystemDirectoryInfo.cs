using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sourcery.IO.FileSystem
{
    public class FileSystemDirectoryInfo : IDirectory, IRootDirectory
    {
        private readonly FileSystemFs _fs;
        private readonly System.IO.DirectoryInfo _directoryInfo;

        public FileSystemDirectoryInfo(FileSystemFs fs, string path) : this(fs, new System.IO.DirectoryInfo(path))
        {
        }

        private FileSystemDirectoryInfo(FileSystemFs fs, System.IO.DirectoryInfo directoryInfo)
        {
            _fs = fs;
            _directoryInfo = directoryInfo;
        }

        public IEnumerable<IDirectory> EnumerateDirectories(string fileName)
        {
            return _directoryInfo.EnumerateDirectories().Select(di => new FileSystemDirectoryInfo(_fs, di));
        }

        public void Create()
        {
            _directoryInfo.Create();
        }

        public IEnumerable<FileInfo> EnumerateFiles(string pattern = null)
        {
            return _directoryInfo.EnumerateFiles().Select(fi => new FileSystemFileInfo(fi));
        }

        public bool Exists
        {
            get { return _directoryInfo.Exists; }
        }

     

        public string Name
        {
            get { return _directoryInfo.Name; }
        }
         
        public Fs Fs
        {
            get { return _fs; }
        }

        public IDirectorySession OpenSession()
        {
            return new FileSystemDirectorySession(this);
        }

        public IDirectory GetDirectoryInfo(string key)
        {
            return new FileSystemDirectoryInfo(_fs, new DirectoryInfo(Path.Combine(_directoryInfo.FullName, key)));
        }

        public class FileSystemDirectorySession : IDirectorySession
        {
            private readonly FileSystemDirectoryInfo _fileSystemDirectoryInfo;

            public FileSystemDirectorySession(FileSystemDirectoryInfo fileSystemDirectoryInfo)
            {
                _fileSystemDirectoryInfo = fileSystemDirectoryInfo;

                if (_fileSystemDirectoryInfo._directoryInfo.EnumerateFiles("*.next").Any())
                    throw new Exception(fileSystemDirectoryInfo.Name + " contains .next files indicating that a save went wrong! needs cleanup");

                if (_fileSystemDirectoryInfo._directoryInfo.EnumerateFiles("*.prev").Any())
                    throw new Exception(fileSystemDirectoryInfo.Name + " contains .prev files indicating that a save went wrong! needs cleanup");
                
            }

            public void Dispose()
            {
                foreach (var next in _fileSystemDirectoryInfo._directoryInfo.EnumerateFiles("*.next"))
                {
                    next.Delete();
                }
            }

            public void Write(string filename, string content)
            {
                File.WriteAllText(Path.Combine(_fileSystemDirectoryInfo._directoryInfo.FullName, filename + ".next"), content);
            }

            public void Save()
            {
                foreach (var file in _fileSystemDirectoryInfo._directoryInfo.GetFiles("*.next"))
                {
                    var originalName = file.FullName.Substring(0, file.FullName.Length - 5);
                    if (File.Exists(originalName))
                    {
                        file.MoveTo(originalName + ".prev"); //backup the previous file
                    }
                    file.CopyTo(originalName, true); //replace it
                }
            }

            public IEnumerable<FileInfo> EnumerateFiles(string searchPattern)
            {
                return _fileSystemDirectoryInfo.EnumerateFiles(searchPattern)
                    .Where(fi => fi.Name.EndsWith(".prev") == false);
            }

            public string ReadAllText(FileInfo fi)
            {
                return File.ReadAllText(Path.Combine(_fileSystemDirectoryInfo._directoryInfo.FullName, fi.Name));
            }
        }
    }
}