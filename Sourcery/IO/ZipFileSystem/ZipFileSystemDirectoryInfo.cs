using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace Sourcery.IO.ZipFileSystem
{
    public class ZipFileSystemDirectoryInfo : IDirectory
    {
        private readonly ZipFileSystemFs _fs;
        private readonly ZipFileRootDirectory _parent;
        private readonly string _path;

        public ZipFileSystemDirectoryInfo(ZipFileSystemFs fs, ZipFileRootDirectory parent, string path)
        {
            _fs = fs;
            _parent = parent;
            _path = path;
        }




        public void Create()
        {
            if (!Exists)
            {
                using (var zipFile = new ZipFile())
                {
                    zipFile.Save(_path);
                }
            }
        }




        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
            Replace("\\*", ".*").
            Replace("\\?", ".") + "$";
        }

        public bool Exists
        {
            get { return File.Exists(_path); }
        }


        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(_path); }
        }


        public Fs Fs
        {
            get { return _fs; }
        }

        public IDirectorySession OpenSession()
        {
            return new DirectorySession(this);
        }

        public class DirectorySession : IDirectorySession
        {
            private readonly ZipFileSystemDirectoryInfo _zipFileSystemDirectoryInfo;
            private ZipFile zip;

            public DirectorySession(ZipFileSystemDirectoryInfo zipFileSystemDirectoryInfo)
            {
                _zipFileSystemDirectoryInfo = zipFileSystemDirectoryInfo;
                _zipFileSystemDirectoryInfo.Create();
                zip = ZipFile.Read(_zipFileSystemDirectoryInfo._path);
            }


            public void Write(string filename, string content)
            {
                zip.UpdateEntry(filename, content, Encoding.UTF8);
            }

            public void Save()
            {
                zip.Save();
            }

            public IEnumerable<FileInfo> EnumerateFiles(string pattern)
            {
                var regex = new Regex(WildcardToRegex(pattern ?? "*"));
                return zip.EntryFileNames.OrderBy(n => n).ToArray()
                    .Where(n => regex.IsMatch(Path.GetFileName(n)))
                    .Select(n => zip[n])
                    .Where(ze => ze.IsDirectory == false)
                    .Select(z => new ZipFileSystemFileInfo(_zipFileSystemDirectoryInfo._fs, _zipFileSystemDirectoryInfo._path, z.FileName));
            }

            public string ReadAllText(FileInfo fi)
            {
                using (var stream = zip[fi.Name].OpenReader())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }

            public void Delete()
            {
                this._zipFileSystemDirectoryInfo.Delete();
            }

            public int Count(string searchPattern)
            {
                return EnumerateFiles(searchPattern).Count();

            }


            public void Dispose()
            {
                zip.Dispose();
            }
        }

        public void Delete()
        {
            File.Delete(_path);
        }
    }
}