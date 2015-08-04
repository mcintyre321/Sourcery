namespace Sourcery.IO.FileSystem
{
    public class FileSystemFileInfo : FileInfo
    {
        private readonly System.IO.FileInfo _fi;

        public FileSystemFileInfo(System.IO.FileInfo fi)
        {
            _fi = fi;
        }

        public override string Name
        {
            get { return _fi.Name; }
        }
    }
}