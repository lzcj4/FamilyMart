using System;
using System.Collections.Generic;
using FileExplorer.Model;

namespace FileExplorer.Factory
{
    abstract class ExplorerFactoryBase
    {
        public abstract void GetRootFoldersAsync(Action<IEnumerable<IFolder>> callback);
    }
}
