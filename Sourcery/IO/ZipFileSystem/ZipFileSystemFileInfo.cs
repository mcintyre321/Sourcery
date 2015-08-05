using Sourcery.EventStores;

namespace Sourcery.IO.ZipFileSystem
{
    public class ZipFileSystemFileInfo : FileInfo
    {
        public ZipFileSystemFileInfo(string fileName)
        {
            Name = fileName;
        }

        public override string Name { get; }
    }
}