using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileExplorer.Helper;

namespace FileExplorer.Model
{
    public class LocalFolder : FolderBase
    {
        public static readonly IFolder PlackHolderItem = new LocalFolder();

        protected LocalFolder()
        {
        }

        public LocalFolder(string path, IFolder parent)
            : base(path, parent)
        {
            /// CD-ROM is not existed 
            /// Virtual folder is not existed
            //if (!this.FullPath.IsNullOrEmpty() && Directory.Exists(this.FullPath))
            if (this.FullPath.IsNullOrEmpty())
            {
                return;
            }

            this.AddPlaceHolder();

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(this.FullPath);
                if (!dirInfo.IsNull())
                {
                    this.fileAttr = dirInfo.Attributes;
                    this.Name = dirInfo.Name;
                    this.LastModifyTime = dirInfo.LastWriteTime;
                }
                ///Pre-load network driver icon 
                ///else will block UI
                //var icon = this.Icon;
            }
            catch (Exception ex)
            {
                LogHelper.Debug("Local folder constructor:{0}", ex);
            }
        }


        public LocalFolder(DirectoryInfo dirInfo, IFolder parent)
            : base(dirInfo.FullName, parent)
        {
            if (dirInfo.IsNull())
            {
                throw new ArgumentNullException();
            }

            this.AddPlaceHolder();
            string fullPath = dirInfo.FullName;
            if (!this.Parent.IsNull() && !this.Parent.IsEnabled)
            {
                this.IsEnabled = this.Parent.IsEnabled;
            }

            this.fileAttr = dirInfo.Attributes;
            this.Name = dirInfo.Name;
            this.LastModifyTime = dirInfo.LastWriteTime;
            ///Pre-load network driver icon 
            ///else will block UI
            //var icon = this.Icon;
        }

        private const int chunk = 300;
        protected override int Chunk
        {
            get
            {
                return chunk;
            }
        }

        #region Sync Folder or file  query

        protected virtual IEnumerable<IFolder> GetFolders()
        {
            if (!IsFolderLoaded)
            {
                RunOnUIThread(() =>
                {
                    this.Folders.Clear();
                });

                IEnumerable<IFolder> folders = SearchFolders();
                IsFolderLoaded = AddItemsByChunk(folders, this.Folders, this.Items);
            }
            return this.Folders;
        }

        protected const int folderPathMaxLen = 248;
        protected const int filePathMaxLen = 260;

        private IEnumerable<IFolder> SearchFolders(string searchPattern = searchAllWildChar)
        {
            IEnumerable<IFolder> result = new IFolder[0];
            return result;
        }

        protected virtual IEnumerable<IFile> GetFiles()
        {
            if (!IsFileLoaded)
            {
                IEnumerable<IFile> files = SearchFiles();
                IsFileLoaded = AddItemsByChunk(files, this.Files, this.Items);
            }
            return Files;
        }

        private IEnumerable<IFile> SearchFiles(string searchPattern = searchAllWildChar)
        {
            IEnumerable<IFile> result = new IFile[0];
            return result;
        }

        protected override void AddPlaceHolder()
        {
            if (this != PlackHolderItem &&
                !this.Folders.Contains(PlackHolderItem))
            {
                RunOnUIThread(() =>
                {
                    this.Folders.Add(PlackHolderItem);
                });
            }
        }

        #endregion

        #region Async interface

        public override void GetFoldersAsync(Action<IEnumerable<IFolder>> callback)
        {
            Action action = () =>
            {
                lock (lockObj)
                {
                    if (!IsFolderLoaded)
                    {
                        try
                        {
                            GetFolders();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(string.Format("GetFoldersAsync failed:{0}", ex.Message));
                        }
                    }
                }
            };

            action.BeginInvoke(ar =>
            {
                EndInvokeAction(ar);
                if (!callback.IsNull())
                {
                    callback(this.Folders);
                }
            }, action);
        }

        public override void GetFilesAsync(Action<IEnumerable<IFile>> callback)
        {
            Action action = () =>
            {
                lock (lockObj)
                {
                    if (!IsFileLoaded)
                    {
                        GetFiles();
                    }
                }
            };

            action.BeginInvoke(ar =>
            {
                EndInvokeAction(ar);
                if (!callback.IsNull())
                {
                    callback(this.Files);
                }
            }, action);
        }

        #endregion
        public override object Clone()
        {
            LocalFolder folder = new LocalFolder();
            CloneMembers(folder);
            return folder;
        }

    }
}
