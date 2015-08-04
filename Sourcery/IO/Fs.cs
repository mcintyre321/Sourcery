using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sourcery.IO.FileSystem;

namespace Sourcery.IO
{
    public abstract class Fs
    {
        public abstract IRootDirectory Root { get; }
    }
}
