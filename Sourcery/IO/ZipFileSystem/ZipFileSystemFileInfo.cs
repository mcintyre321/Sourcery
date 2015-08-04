using Sourcery.EventStores;

namespace Sourcery.IO.ZipFileSystem
{
    public class ZipFileSystemFileInfo : FileInfo
    {
        private readonly ZipFileSystemFs _fs;
        private readonly string _zipFilePath;
        private readonly string _fileName;



        public ZipFileSystemFileInfo(ZipFileSystemFs fs, string zipFilePath, string fileName)
        {
            _fs = fs;
            _zipFilePath = zipFilePath;
            _fileName = fileName;
        }

        public override string Name
        {
            get { return _fileName; }
        }
    }
}