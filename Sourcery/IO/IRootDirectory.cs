using System.Collections.Generic;

namespace Sourcery.IO
{
    public interface IRootDirectory
    {
        IDirectory GetDirectoryInfo(string key);
        IEnumerable<IDirectory> EnumerateDirectories(string pattern = null);
    }
}