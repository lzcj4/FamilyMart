using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileExplorer.Helper;
using FileExplorer.Model;

namespace FileExplorer.Factory
{
    class LocalExplorerFactory : ExplorerFactoryBase
    {
        public override void GetRootFoldersAsync(Action<IEnumerable<IFolder>> callback)
        {
            ObservableCollection<IFolder> roots = new ObservableCollection<IFolder>();

            LocalRootFolder pcFolder = new LocalRootFolder();
            roots.Add(pcFolder);


            if (!callback.IsNull())
            {
                callback(roots);
            }
        }
    }
}
