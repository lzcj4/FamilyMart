using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DAL.Model;
using FamilyMartUI.Common;
using FamilyMartUI.ViewModel;

namespace FamilyMartUI.UC
{
    /// <summary>
    /// Interaction logic for UCContent.xaml
    /// </summary>
    public partial class UCReport : UserControl
    {
        #region DP

        /// <summary>
        ///Content view
        /// </summary>
        public static readonly DependencyProperty ContentViewProperty =
            DependencyProperty.Register("ContentView", typeof(ICollectionView), typeof(UCReport));

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
        ///SummaryText
        /// </summary>
        public static readonly DependencyProperty SummaryTextProperty =
            DependencyProperty.Register("SummaryText", typeof(string), typeof(UCReport));

        public static void SetSummaryText(DependencyObject element, string value)
        {
            if (element == null) return;
            element.SetValue(SummaryTextProperty, value);
        }

        public static string GetSummaryText(DependencyObject element)
        {
            if (element == null) return string.Empty;
            return (string)element.GetValue(SummaryTextProperty);
        }

        #endregion

        ISortOrder ViewModel
        {
            get
            {
                return this.DataContext as ISortOrder;
            }
        }

        public UCReport()
        {
            InitializeComponent();
            lvContent.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
            this.Loaded += (sender, e) =>
            {
                this.SetSortOrderCallback();
            };
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
                lastHeader = ColSaleDateHeader;
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
            if (!lastHeader.IsNull() && lastHeader != ColSaleDateHeader)
            {
                lastHeader.ContentTemplate = NormalHeaderDataTemplate;
            }
            lastHeader = ColSaleDateHeader;
            if (!lastHeader.IsNull())
            {
                lastHeader.ContentTemplate = AscHeaderDataTemplate;
                lastDirection = ListSortDirection.Ascending;
            }
        }

        GridViewColumnHeader colSaleDateHeader = null;
        private GridViewColumnHeader ColSaleDateHeader
        {
            get
            {
                if (colSaleDateHeader.IsNull())
                {
                    var allHeaders = GetVisualChildren<GridViewColumnHeader>(this.lvContent);
                    foreach (var item in allHeaders)
                    {
                        if (!item.Column.IsNull() &&
                            ColumnWidthHelper.GetSortPropertyName(item.Column) == "SaleDate")
                        {
                            colSaleDateHeader = item;
                            break;
                        }
                    }
                }
                return colSaleDateHeader;
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

            ResourceDictionary resDic = Application.Current.Resources;
            if (resDic.Contains(resName))
            {
                result = resDic[resName];
            }
            return result;
        }

        #endregion

        public event EventHandler<EventArgs<DialyReport>> OnSelectionChanged;
        private void lvContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DialyReport item = lvContent.SelectedItem as DialyReport;
            if (!OnSelectionChanged.IsNull() && !item.IsNull())
            {
                OnSelectionChanged(this, new EventArgs<DialyReport>(item));
            }
        }
    }

    public class EventArgs<T> : EventArgs
    {
        public T Content { get; protected set; }
        public EventArgs(T item)
        {
            this.Content = item;
        }
    }
    public class EventArgs<T1, T2> : EventArgs
    {
        public T1 Item1 { get; protected set; }
        public T2 Item2 { get; protected set; }
        public EventArgs(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
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

    }
}
