using System;
using System.Collections.Generic;

namespace Sourcery.IO
{
    public interface IDirectory
    {
        void Create();
        string Name { get; }
        IDirectorySession OpenSession();
        void Delete();
    }

    public interface IDirectorySession : IDisposable
    {
        void Write(string filename, string content);
        void Save();
        IEnumerable<FileInfo> EnumerateFiles(string searchPattern);
        string ReadAllText(FileInfo fi);
        void Delete();
        int Count(string searchPattern);
    }
}