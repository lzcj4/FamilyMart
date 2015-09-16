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
        internal const FileAttributes ExcludeFileAttributes = FileAttributes.Hidden | FileAttributes.System;//| FileAttributes.Temporary;
        internal static List<string> LocalExcludedFolders = new List<string>();

        static LocalFolder()
        {
            var str = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string progPath = str.IsNullOrEmpty() ? string.Empty : str.ToLower();

            str = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string progX86Path = str.IsNullOrEmpty() ? string.Empty : str.ToLower();

            str = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string windPath = str.IsNullOrEmpty() ? string.Empty : str.ToLower();

            str = Environment.GetEnvironmentVariable("ProgramW6432");
            string progW6432 = str.IsNullOrEmpty() ? string.Empty : str.ToLower();

            if (!windPath.IsNullOrEmpty())
            {
                LocalExcludedFolders.Add(windPath);
            }

            if (!progPath.IsNullOrEmpty())
            {
                LocalExcludedFolders.Add(progPath);
            }

            if (!progW6432.IsNullOrEmpty() && !LocalExcludedFolders.Contains(progW6432))
            {
                LocalExcludedFolders.Add(progW6432);
            }

            if (!progX86Path.IsNullOrEmpty() && !LocalExcludedFolders.Contains(progX86Path))
            {
                LocalExcludedFolders.Add(progX86Path);
            }
        }

        public static bool GetIsExcludeFolder(string path)
        {
            bool result = false;
            if (!path.IsNullOrEmpty())
            {
                result = LocalExcludedFolders.Contains(path.ToLower());
            }
            return result;
        }

        public static bool GetIsExcludeAttribute(FileAttributes attrs)
        {
            bool result = (attrs & ExcludeFileAttributes) > 0;
            return result;
        }

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
            this.IsEnabled = !LocalExcludedFolders.Contains(path.ToLower());
            if (!this.Parent.IsNull() && !this.Parent.IsEnabled)
            {
                this.IsEnabled = this.Parent.IsEnabled;
            }

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
            this.IsEnabled = !LocalExcludedFolders.Contains(fullPath.ToLower());
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

            ///Check is folder is existed before query
            ///CD-ROM will block query for a while without check existed
            try
            {
                //The specified path, file name, or both are too long. 
                //The fully qualified file name must be less than 260 characters, 
                //and the directory name must be less than 248 characters.
                if (Directory.Exists(this.FullPath) && this.FullPath.Length < folderPathMaxLen)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(this.FullPath);
                    result = dirInfo.GetDirectories().Where(item =>
                    {
                        try
                        {
                            return item.FullName.Length < folderPathMaxLen && (item.Attributes & ExcludeFileAttributes) == 0;
                        }
                        catch (Exception ex)
                        {
                            ///Access denied exception
                            LogHelper.Debug(ex);
                            return false;
                        }
                    }).Select(item => new LocalFolder(item, this));
                    result = SetFolderOrder(result);
                }
            }
            catch (Exception ex)
            {
                ///Access denied exception
                LogHelper.Debug(ex);
            }
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
            try
            {
                //The specified path, file name, or both are too long. 
                //The fully qualified file name must be less than 260 characters, 
                //and the directory name must be less than 248 characters.
                if (Directory.Exists(this.FullPath) && this.FullPath.Length < folderPathMaxLen)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(this.FullPath);
                    result = dirInfo.GetFiles().Where(item =>
                    {
                        try
                        {
                            return item.FullName.Length < filePathMaxLen && (item.Attributes & ExcludeFileAttributes) == 0;
                        }
                        catch (Exception ex)
                        {
                            ///Access denied exception
                            LogHelper.Debug(ex);
                            return false;
                        }
                    }).Select(item => new LocalFile(item, this));

                    result = SetFolderOrder(result);
                }
            }
            catch (Exception ex)
            {
                ///Access denied exception
                LogHelper.Debug(ex);
            }
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
