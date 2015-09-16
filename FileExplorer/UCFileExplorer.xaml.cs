using FileExplorer.Factory;
using FileExplorer.Helper;
using FileExplorer.Model;
using FileExplorer.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for UCFileExplorer.xaml
    /// </summary>
    public partial class UCFileExplorer : UserControl
    {
        #region DP

        /// <summary>
        ///Is Search enabled
        /// </summary>
        public static readonly DependencyProperty IsSearchEnabledProperty =
            DependencyProperty.Register("IsSearchEnabled", typeof(bool), typeof(UCFileExplorer));

        public static void SetIsSearchEnabled(DependencyObject element, bool value)
        {
            if (element == null)
                return;
            element.SetValue(IsSearchEnabledProperty, value);
        }

        public static bool GetIsSearchEnabled(DependencyObject element)
        {
            if (element == null)
                return false;
            return (bool)element.GetValue(IsSearchEnabledProperty);
        }

        #endregion

        public FileExplorerViewModel ViewModel
        {
            get
            {
                return this.DataContext as FileExplorerViewModel;
            }
        }

        public UCFileExplorer()
        {
            InitializeComponent();
            this.ucContent.ItemClicked += ucContent_ItemClicked;
        }

        void ucContent_ItemClicked(object sender, ContentEventArgs<Model.IFile> e)
        {
            IFile item = e.Content;
            if (item.IsNull() || this.ViewModel.IsNull())
            {
                return;
            }

            if (item is IFolder)
            {
                IFolder folder = item as IFolder;
                folder.IsExpanded = true;

                IFolder parentFolder = folder.Parent;
                while (!parentFolder.IsNull())
                {
                    parentFolder.IsExpanded = true;
                    if (parentFolder == parentFolder.Parent)
                    {
                        break;
                    }
                    parentFolder = parentFolder.Parent;
                }

                folder.IsSelected = true;
                this.ViewModel.SetCurrentFolder(folder);
            }
            else if (item is LocalFile)
            {
                if (File.Exists(item.FullPath))
                {
                    try
                    {
                        Process.Start(item.FullPath);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Debug("Exception:", ex);
                    }
                }
            }
        }
    }
}
