using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileExplorer.Factory;
using FileExplorer.Helper;

namespace FileExplorer.Model
{
    public class LocalRootFolder : LocalFolder
    {
        DriverWatcher driverWatcher = null;
        /// <summary>
        /// notify drive change
        /// </summary>
        public event DriverChangedEventhandler DriverChanged;

        public LocalRootFolder()
            : base(string.Empty, null)
        {
            this.IsCheckVisible = false;
            this.IsExpanded = true;
            this.IsSelected = true;
            this.Parent = this;
        }

        protected override IEnumerable<IFolder> GetFolders()
        {
            if (!IsFolderLoaded)
            {

            }
            return this.Folders;
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);
        }
    }
}
