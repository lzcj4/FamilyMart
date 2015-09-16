using System;
using System.IO;
using FileExplorer.Helper;

namespace FileExplorer.Model
{
    public class LocalFile : FileBase
    {
        public LocalFile(string path, IFolder parent)
            : base(path, parent)
        {
            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(this.FullPath);
            }
            catch (IOException ex)
            {
                LogHelper.Debug("Local folder constructor:{0}", ex);
            }

            if (fileInfo.IsNull())
            {
                return;
            }
            this.Name = fileInfo.Name;
            this.Extension = fileInfo.Extension;
            this.Size = fileInfo.Length;
            this.LastModifyTime = fileInfo.LastWriteTime;
            this.fileAttr = fileInfo.Attributes;
            ///Pre-load network driver icon 
            ///else will block UI
            //var icon = this.Icon;
        }

        public LocalFile(FileInfo fi, IFolder parent)
            : base(fi.FullName, parent)
        {
            if (fi.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.Name = fi.Name;
            this.Extension = fi.Extension;
            try
            {
                this.Size = fi.Length;
                this.LastModifyTime = fi.LastWriteTime;
                this.fileAttr = fi.Attributes;
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("Local file initial failed:{0}", ex.Message);
            }
        }

        public override object Clone()
        {
            LocalFile file = new LocalFile(this.FullPath, this.Parent);
            CloneMembers(file);
            return file;
        }
    }
}
