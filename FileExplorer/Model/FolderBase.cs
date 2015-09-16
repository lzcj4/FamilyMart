using FileExplorer.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileExplorer.Model
{
    public abstract class FolderBase : FileBase, IFolder
    {
        protected const string searchAllWildChar = "*";
        protected object lockObj = new object();

        #region IFolder properties

        public override string Title
        {
            get
            {
                return title.IsNullOrEmpty() ? this.Name : title;
            }
            protected set
            {
                SetProperty(ref title, value, "Title");
            }
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            protected set
            {
                SetProperty(ref isLoading, value, "IsLoading");
            }
        }

        private bool isExpanded = false;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                SetProperty(ref isExpanded, value, "IsExpanded");
            }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                SetProperty(ref isSelected, value, "IsSelected");
            }
        }
        
        public bool IsCanceled
        {
            get;
            protected set;
        }

        private IFolder virtualParent = null;
        public IFolder VirtualParent
        {
            get { return virtualParent; }
            set
            {
                SetProperty(ref virtualParent, value, "VirtualParent");
            }
        }

        #endregion

        public FolderBase(string path, IFolder parent)
        {
            this.FullPath = path;
            this.Parent = parent;
            fileAttr = FileAttributes.Directory;
        }

        protected FolderBase()
        {
            fileAttr = FileAttributes.Directory;
        }

        private ObservableCollection<IFolder> folders = new ObservableCollection<IFolder>();
        /// <summary>
        /// Sub folders
        /// </summary>
        public ObservableCollection<IFolder> Folders
        {
            get { return folders; }
        }

        private ObservableCollection<IFile> files = new ObservableCollection<IFile>();
        /// <summary>
        /// Sub files
        /// </summary>
        public ObservableCollection<IFile> Files
        {
            get { return files; }
        }

        private ObservableCollection<IFile> items = new ObservableCollection<IFile>();
        /// <summary>
        /// Sub folders and files
        /// </summary>
        public ObservableCollection<IFile> Items
        {
            get { return items; }
        }

        private bool isFolderLoaded = false;
        /// <summary>
        /// Is children folder loaded
        /// </summary>
        protected bool IsFolderLoaded
        {
            get { return isFolderLoaded; }
            set { isFolderLoaded = value; }
        }

        private bool isFileLoaded = false;
        /// <summary>
        /// Is children files loaded
        /// </summary>
        protected bool IsFileLoaded
        {
            get { return isFileLoaded; }
            set { isFileLoaded = value; }
        }


        public virtual void Cancel()
        {
            this.IsCanceled = true;
            this.IsLoading = false;
            this.Clear();
        }

        /// <summary>
        /// Clear files
        /// </summary>
        protected void Clear()
        {
            ///BIUPC-2265: collection list operate in other thread
            ///Due to add lock in RunOnUIThread , this issue should be resolved

            if (!this.IsFolderLoaded)
            {
                RunOnUIThread(() =>
                {
                    try
                    {
                        foreach (var item in this.Folders)
                        {
                            item.Dispose();
                            this.Items.Remove(item);
                        }
                        this.Folders.Clear();
                        this.AddPlaceHolder();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("FolderBase.Clear error:" + ex.Message);
                    }
                });
            }

            if (!this.IsFileLoaded)
            {
                RunOnUIThread(() =>
                        {
                            try
                            {
                                foreach (var item in this.Files)
                                {
                                    this.Items.Remove(item);
                                }
                                this.Files.Clear();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("FolderBase.Clear error:" + ex.Message);
                            }
                        });
            }

        }

        protected abstract void AddPlaceHolder();

        private const int chunk = 100;
        protected virtual int Chunk
        {
            get
            {
                return chunk;
            }
        }

        protected bool AddItemsByChunk<T>(IEnumerable<T> source, params IList[] destinations) where T : IFile
        {
            if (source.IsNullOrEmpty() || destinations.IsNull())
            {
                return true;
            }

            int index = 0;
            int getCount = Chunk;

            while (getCount > 0)
            {
                Stopwatch swg = new Stopwatch();
                swg.Start();

                var chunkItems = source.Skip(index++ * Chunk).Take(Chunk);
                getCount = chunkItems.Count();
                if (IsCanceled)
                {
                    Clear();
                    return false;
                }

                swg.Stop();
                Debug.WriteLine("----- get files:{0} ,elapsed:{1} ms", getCount, swg.ElapsedMilliseconds);

                if (getCount == 0)
                {
                    break;
                }

                Stopwatch swa = new Stopwatch();
                swa.Start();

                RunOnUIThread(() =>
                {
                    foreach (var item in chunkItems)
                    {
                        if (IsCanceled)
                        {
                            return;
                        }
                        foreach (var list in destinations)
                        {
                            list.Add(item);
                        }
                    }
                });

                swa.Stop();
                Debug.WriteLine("+++++ add files:{0} ,elapsed:{1} ms", getCount, swa.ElapsedMilliseconds);
            }
            return true;
        }

        #region IFolderAsync

        public virtual void GetChildrenAsync(Action<IEnumerable<IFile>> callback)
        {
            IsCanceled = false;

            ///If all subitem got, don't need show loading
            if (!IsFolderLoaded || !IsFileLoaded)
            {
                IsLoading = true;
            }

            this.GetFoldersAsync((folders) =>
            {
                this.GetFilesAsync((files) =>
                {
#if DEBUG
                    if (!IsCanceled)
                    {
                        Debug.Assert(IsFolderLoaded && IsFileLoaded, "---- GetChildrenAsync callback conditions not all true");
                    }
#endif
                    IsLoading = false;
                    if (!callback.IsNull())
                    {
                        callback(this.Items);
                    }
                });
            });
        }

        protected void EndInvokeAction(IAsyncResult result)
        {
            Action action = result.AsyncState as Action;
            if (action.IsNull())
            {
                return;
            }
            action.EndInvoke(result);
        }

        public abstract void GetFoldersAsync(Action<IEnumerable<IFolder>> callback);

        public abstract void GetFilesAsync(Action<IEnumerable<IFile>> callback);

        public void GetItemAsync(string path, Action<IFile> callback, bool isRecursive = true)
        {
            if (path.IsNullOrEmpty() ||
                (!this.FullPath.IsNullOrEmpty() && !path.StartWithIgnoreCase(FullPath)))
            {
                if (!callback.IsNull())
                {
                    callback(null);
                }
                return;
            }

            if (path.EqualIgnoreCase(this.FullPath))
            {
                if (!callback.IsNull())
                {
                    callback(this);
                }
                return;
            }

            IFile result = null;
            if (!isRecursive)
            {
                this.GetChildrenAsync((items) =>
                {
                    try
                    {
                        result = items.FirstOrDefault(f => f.FullPath.EqualIgnoreCase(path));
                    }
                    catch (Exception ex)
                    {
                    }

                    if (!callback.IsNull())
                    {
                        callback(result);
                    }
                });
                return;
            }

            ///Get folder recursively
            const string pathFormat = "{0}{1}";
            const StringSplitOptions splitOpt = StringSplitOptions.RemoveEmptyEntries;
            char pathSepChar = Path.DirectorySeparatorChar;
            char[] pathSepChars = new[] { pathSepChar };

            string[] parts = null;
            if (this.FullPath.IsNullOrEmpty())
            {
                parts = path.Split(pathSepChars, splitOpt);
            }
            else if (this.FullPath == pathSepChar.ToString())
            {
                ///Remote root folder full path is \
                parts = path.Substring(path.IndexOf(this.FullPath) + 1).Split(pathSepChars, splitOpt);
            }
            else
            {
                parts = path.Replace(this.FullPath, string.Empty).Split(pathSepChars, splitOpt);
            }

            if (parts.IsNullOrEmpty())
            {
                if (!callback.IsNull())
                {
                    callback(null);
                }
                return;
            }

            string newPath = FullPath;

            if (newPath.IsNullOrEmpty())
            {
                ///The driver path,end with '\'
                newPath = string.Format(pathFormat, parts[0], pathSepChar);
            }
            else
            {
                newPath = Path.Combine(newPath, parts[0]);
            }

            this.GetChildrenAsync((items) =>
            {
                IFile item = null;
                try
                {
                    item = items.FirstOrDefault(f => f.FullPath.EqualIgnoreCase(newPath));
                }
                catch
                {
                }

                if (!item.IsNull())
                {
                    if (item.FullPath.EqualIgnoreCase(path))
                    {
                        if (!callback.IsNull())
                        {
                            callback(item);
                            return;
                        }
                    }

                    if (item is IFolder)
                    {
                        (item as IFolder).GetItemAsync(path, callback, isRecursive);
                    }
                }
                else if (!callback.IsNull())
                {
                    callback(null);
                    return;
                }
            });
        }

        public static IEnumerable<T> SetFolderOrder<T>(IEnumerable<T> list, bool isAsc = true) where T : IFile
        {
            if (list.IsNullOrEmpty())
            {
                return list;
            }

            if (isAsc)
            {
                return list.OrderBy(item => item.Name);
            }
            else
            {
                return list.OrderByDescending(item => item.Name);
            }
        }

        public static IEnumerable<T> SetFileOrder<T>(IEnumerable<T> list, bool isAsc = true) where T : IFile
        {
            if (list.IsNullOrEmpty())
            {
                return list;
            }

            if (isAsc)
            {
                return list.OrderBy(item => item.Name);//.ThenBy(item => item.Name.Length);
            }
            else
            {
                return list.OrderByDescending(item => item.Name);//.ThenByDescending(item => item.Name.Length);
            }
        }

        #endregion

        protected override void OnDisposing(bool isDisposing)
        {
            this.Cancel();
            RunOnUIThread(() =>
            {
                this.Items.Clear();
            });
            this.Parent = null;
        }
    }
}
