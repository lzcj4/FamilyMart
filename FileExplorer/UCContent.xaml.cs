﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using FileExplorer.Helper;
using FileExplorer.Model;
using FileExplorer.ViewModel;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for UCContent.xaml
    /// </summary>
    public partial class UCContent : UserControl
    {
        #region DP

        /// <summary>
        ///Content view
        /// </summary>
        public static readonly DependencyProperty ContentViewProperty =
            DependencyProperty.Register("ContentView", typeof(ICollectionView), typeof(UCContent));

        public static void SetContentView(DependencyObject element, ICollectionView value)
        {
            if (element == null) return;
            element.SetValue(ContentViewProperty, value);
        }

        public static ICollectionView GetContentView(DependencyObject element)
        {
            if (element == null) return null;
            return (ICollectionView)element.GetValue(ContentViewProperty);
        }

        /// <summary>
        ///Is Loading
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(UCContent));

        public static void SetIsLoading(DependencyObject element, bool value)
        {
            if (element == null) return;
            element.SetValue(IsLoadingProperty, value);
        }

        public static bool GetIsLoading(DependencyObject element)
        {
            if (element == null) return false;
            return (bool)element.GetValue(IsLoadingProperty);
        }

        /// <summary>
        ///Is content empty
        /// </summary>
        public static readonly DependencyProperty IsEmptyProperty =
            DependencyProperty.Register("IsEmpty", typeof(bool), typeof(UCContent));

        public static void SetIsEmpty(DependencyObject element, bool value)
        {
            if (element == null) return;
            element.SetValue(IsEmptyProperty, value);
        }

        public static bool GetIsEmpty(DependencyObject element)
        {
            if (element == null) return false;
            return (bool)element.GetValue(IsEmptyProperty);
        }

        /// <summary>
        ///Is Error
        /// </summary>
        public static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register("IsError", typeof(bool), typeof(UCContent));

        public static void SetIsError(DependencyObject element, bool value)
        {
            if (element == null) return;
            element.SetValue(IsErrorProperty, value);
        }

        public static bool GetIsError(DependencyObject element)
        {
            if (element == null) return false;
            return (bool)element.GetValue(IsErrorProperty);
        }

        /// <summary>
        ///Empty hint
        /// </summary>
        public static readonly DependencyProperty EmptyHintProperty =
            DependencyProperty.Register("EmptyHint", typeof(string), typeof(UCContent));

        public static void SetEmptyHint(DependencyObject element, string value)
        {
            if (element == null) return;
            element.SetValue(EmptyHintProperty, value);
        }

        public static string GetEmptyHint(DependencyObject element)
        {
            if (element == null) return string.Empty;
            return (string)element.GetValue(EmptyHintProperty);
        }

        /// <summary>
        ///Error hint
        /// </summary>
        public static readonly DependencyProperty ErrorHintProperty =
            DependencyProperty.Register("ErrorHint", typeof(string), typeof(UCContent));

        public static void SetErrorHint(DependencyObject element, string value)
        {
            if (element == null) return;
            element.SetValue(ErrorHintProperty, value);
        }

        public static string GetErrorHint(DependencyObject element)
        {
            if (element == null) return string.Empty;
            return (string)element.GetValue(ErrorHintProperty);
        }


        /// <summary>
        ///Search folder width
        /// </summary>
        public static readonly DependencyProperty FolderPathWidthProperty =
            DependencyProperty.Register("FolderPathWidth", typeof(double), typeof(UCContent));

        public static void SetFolderPathWidth(DependencyObject element, double value)
        {
            if (element == null) return;
            element.SetValue(FolderPathWidthProperty, value);
        }

        public static double GetFolderPathWidth(DependencyObject element)
        {
            if (element == null) return 0;
            return (double)element.GetValue(FolderPathWidthProperty);
        }

        /// <summary>
        ///Is check all enabled
        /// </summary>
        public static readonly DependencyProperty IsAllCheckedProperty =
            DependencyProperty.Register("IsAllChecked", typeof(bool?), typeof(UCContent));

        public static void SetIsAllChecked(DependencyObject element, bool? value)
        {
            if (element == null) return;
            element.SetValue(IsAllCheckedProperty, value);
        }

        public static bool? GetIsAllChecked(DependencyObject element)
        {
            if (element == null) return false;
            return (bool?)element.GetValue(IsAllCheckedProperty);
        }

        /// <summary>
        ///Is check all enabled
        /// </summary>
        public static readonly DependencyProperty IsAllCheckEnabledProperty =
            DependencyProperty.Register("IsAllCheckEnabled", typeof(bool), typeof(UCContent), new PropertyMetadata(true));

        public static void SetIsAllCheckEnabled(DependencyObject element, bool value)
        {
            if (element == null) return;
            element.SetValue(IsAllCheckEnabledProperty, value);
        }

        public static bool GetIsAllCheckEnabled(DependencyObject element)
        {
            if (element == null) return false;
            return (bool)element.GetValue(IsAllCheckEnabledProperty);
        }

        #endregion

        public event EventHandler<ContentEventArgs<IFile>> ItemClicked;

        ISortOrder ViewModel
        {
            get
            {
                return this.DataContext as ISortOrder;
            }
        }

        public UCContent()
        {
            InitializeComponent();
            lvContent.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
            this.Loaded += (sender, e) =>
            {
                this.SetSortOrderCallback();
            };
        }

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ck = sender as CheckBox;
            if (lvContent.ItemsSource.IsNull() || ck.IsNull())
            { 
                return;
            }

            IFolder folder = ColumnWidthHelper.GetFolder(this);
            //If search view the current folder is null
            if (!folder.IsNull())
            {
                return;
            }

            try
            {
                if (ck.IsChecked == true)
                {
                    foreach (IFile item in lvContent.ItemsSource)
                    {
                        item.IsChecked = true;
                    }
                }
                else if (ck.IsChecked == false)
                {
                    foreach (IFile item in lvContent.ItemsSource)
                    {
                        item.IsChecked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }            
        }

        void ItemCheckBox_Click(object sender, RoutedEventArgs e)
        {
            IFolder folder = ColumnWidthHelper.GetFolder(this);
            //If search view the current folder is null
            if (!folder.IsNull())
            {
                return;
            }

            var list = lvContent.ItemsSource.Cast<IFile>();
            if (list.IsNullOrEmpty())
            {
                return;
            }

            if (list.All(item => item.IsChecked == true))
            {
                SetIsAllChecked(this, true);
            }
            else if (list.All(item => item.IsChecked == false))
            {
                SetIsAllChecked(this, false);
            }
            else
            {
                SetIsAllChecked(this, null);
            }
        }

        void ListViewItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            IFile file = lvContent.SelectedItem as IFile;
            if (file.IsNull())
            {
                return;
            }

            if (e.Key == Key.Space)
            {
                file.IsChecked = file.IsChecked == false;
            }
            else if (e.Key == Key.Enter)
            {
                if (!ItemClicked.IsNull())
                {
                    ItemClicked(this, new ContentEventArgs<IFile>(file));
                }
            }
        }

        /// <summary>
        /// double click folder item to open sub directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lvContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            var colHeader = TryFindParent<GridViewColumnHeader>(e.OriginalSource as DependencyObject);
            if (colHeader != null)
            {
                return;
            }

            IFile item = lvContent.SelectedItem as IFile;
            if (!ItemClicked.IsNull() && !item.IsNull())
            {
                ItemClicked(this, new ContentEventArgs<IFile>(item));
            }

            e.Handled = true;
        }

        private T TryFindParent<T>(DependencyObject current) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(current);

            if (parent == null)
                return null;

            if (parent is T)
                return parent as T;
            else
                return TryFindParent<T>(parent);
        }

        IEnumerable<T> GetVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    yield return (T)child;

                foreach (var descendant in GetVisualChildren<T>(child))
                    yield return descendant;
            }
        }

        #region Header sort

        public void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = e.OriginalSource as Thumb;
            if (thumb.IsNull())
            {
                return;
            }
            GridViewColumnHeader header = thumb.TemplatedParent as GridViewColumnHeader;
            if (header.IsNull())
            {
                return;
            }
            GridViewColumn column = header.Column;
            if (column.IsNull())
            {
                return;
            }

            double minWidth = ColumnWidthHelper.GetMinWidth(column);
            double maxWidth = ColumnWidthHelper.GetMaxWidth(column);

            if (header.Column.ActualWidth < minWidth)
                header.Column.Width = minWidth;
            if (header.Column.ActualWidth > maxWidth)
                header.Column.Width = maxWidth;
        }

        GridViewColumnHeader lastHeader = null;
        ListSortDirection lastDirection = ListSortDirection.Ascending;

        void Header_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader clickHeader = e.OriginalSource as GridViewColumnHeader;
            if (clickHeader.IsNull() || clickHeader.Column.IsNull() ||
                ColumnWidthHelper.GetSortPropertyName(clickHeader.Column).IsNullOrEmpty())
            {
                return;
            }

            ListSortDirection direction = ListSortDirection.Ascending;
            if (clickHeader != lastHeader)
            {
                direction = ListSortDirection.Descending;
            }
            else
            {
                if (lastDirection == ListSortDirection.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    direction = ListSortDirection.Ascending;
                }
            }

            SetSortOrder(clickHeader, direction);
        }

        private void SetSortOrder(GridViewColumnHeader clickHeader, ListSortDirection direction)
        {
            if (lastHeader.IsNull())
            {
                lastHeader = ColNameHeader;
            }
            if (lastHeader.IsNull())
            {
                return;
            }

            string propName = ColumnWidthHelper.GetSortPropertyName(clickHeader.Column);
            if (propName.IsNullOrEmpty() || this.ViewModel.IsNull())
            {
                return;
            }
            ViewModel.SetSortOrder(propName, direction);

            lastHeader.ContentTemplate = NormalHeaderDataTemplate;
            switch (direction)
            {
                case ListSortDirection.Ascending:
                    clickHeader.ContentTemplate = AscHeaderDataTemplate;
                    break;
                case ListSortDirection.Descending:
                    clickHeader.ContentTemplate = DescHeaderDataTemplate;
                    break;
                default:
                    clickHeader.ContentTemplate = NormalHeaderDataTemplate;
                    break;
            }
            lastHeader = clickHeader;
            lastDirection = direction;
        }

        public void SetSortOrderCallback()
        {
            if (this.ViewModel.IsNull())
            {
                return;
            }
            this.ViewModel.SetSortOrderCallback(SortOrderCallback);
            SortOrderCallback();
        }

        public void SortOrderCallback(bool isRefresh = true)
        {
            if (!lastHeader.IsNull() && lastHeader != ColNameHeader)
            {
                lastHeader.ContentTemplate = NormalHeaderDataTemplate;
            }
            lastHeader = ColNameHeader;
            if (!lastHeader.IsNull())
            {
                lastHeader.ContentTemplate = AscHeaderDataTemplate;
                lastDirection = ListSortDirection.Ascending;
            }
        }

        GridViewColumnHeader colNameHeader = null;
        private GridViewColumnHeader ColNameHeader
        {
            get
            {
                if (colNameHeader.IsNull())
                {
                    var allHeaders = GetVisualChildren<GridViewColumnHeader>(this.lvContent);
                    foreach (var item in allHeaders)
                    {
                        if (!item.Column.IsNull() &&
                            ColumnWidthHelper.GetSortPropertyName(item.Column) == FileExplorerViewModel.SortPropertyName)
                        {
                            colNameHeader = item;
                            break;
                        }
                    }
                }
                return colNameHeader;
            }
        }

        private DataTemplate AscHeaderDataTemplate
        {
            get
            {
                return GetHeaderTemplate("AscSortHeaderTemplate") as DataTemplate;
            }
        }

        private DataTemplate DescHeaderDataTemplate
        {
            get
            {
                return GetHeaderTemplate("DescSortHeaderTemplate") as DataTemplate;
            }
        }

        private DataTemplate NormalHeaderDataTemplate
        {
            get
            {
                return GetHeaderTemplate("NormalSortHeaderTemplate") as DataTemplate;
            }
        }

        private object GetHeaderTemplate(string resName)
        {
            object result = null;
            if (resName.IsNullOrEmpty())
            {
                return result;
            }

            if (this.Resources.Contains(resName))
            {
                result = this.Resources[resName];
            }
            return result;
        }

        #endregion
    }

    public class ContentEventArgs<T> : EventArgs
    {
        public bool IsProperty { get; protected set; }
        public T Content { get; protected set; }
        public ContentEventArgs(T item, bool isProperty = true)
        {
            this.Content = item;
            this.IsProperty = isProperty;
        }
    }

    public static class ColumnWidthHelper
    {
        /// <summary>
        /// Min width
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(ColumnWidthHelper));

        public static void SetMinWidth(DependencyObject element, double value)
        {
            if (element == null)
                return;
            element.SetValue(MinWidthProperty, value);
        }

        public static double GetMinWidth(DependencyObject element)
        {
            if (element == null) return 0;
            return (double)element.GetValue(MinWidthProperty);
        }

        /// <summary>
        /// Max width
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.RegisterAttached("MaxWidth", typeof(double), typeof(ColumnWidthHelper));

        public static void SetMaxWidth(DependencyObject element, double value)
        {
            if (element == null)
                return;
            element.SetValue(MaxWidthProperty, value);
        }

        public static double GetMaxWidth(DependencyObject element)
        {
            if (element == null) return 0;
            return (double)element.GetValue(MaxWidthProperty);
        }


        /// <summary>
        /// Sort property name
        /// </summary>
        public static readonly DependencyProperty SortPropertyNameProperty =
            DependencyProperty.RegisterAttached("SortPropertyName", typeof(string), typeof(ColumnWidthHelper));

        public static void SetSortPropertyName(DependencyObject element, string value)
        {
            if (element == null)
                return;
            element.SetValue(SortPropertyNameProperty, value);
        }

        public static string GetSortPropertyName(DependencyObject element)
        {
            if (element == null) return string.Empty;
            return (string)element.GetValue(SortPropertyNameProperty);
        }

        /// <summary>
        /// Max width
        /// </summary>
        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.RegisterAttached("Folder", typeof(IFolder), typeof(ColumnWidthHelper));

        public static void SetFolder(DependencyObject element, IFolder value)
        {
            if (element == null)
                return;
            element.SetValue(FolderProperty, value);
        }

        public static IFolder GetFolder(DependencyObject element)
        {
            if (element == null) return null;
            return (IFolder)element.GetValue(FolderProperty);
        }
    }
}
