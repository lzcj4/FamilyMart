using System.Collections.ObjectModel;
using System.ComponentModel;
using FamilyMartUI.Common;
using System.Windows.Data;

namespace FamilyMartUI.ViewModel
{
    public class SortOrderViewModel<T> : ViewModelBase, ISortOrder where T : new()
    {
        private ObservableCollection<T> items = new ObservableCollection<T>();
        /// <summary>
        /// Root items
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return items; }
        }

        ICollectionView contentView;
        public ICollectionView ContentView
        {
            get { return contentView; }
            protected set
            {
                SetProperty(ref contentView, value, "ContentView");
            }
        }

        public SortOrderViewModel()
        {
            ContentView = CollectionViewSource.GetDefaultView(this.Items);
        }

        #region Sort order

        private SortOrderCallback sortOrderCallback = null;

        public void SetSortOrderCallback(SortOrderCallback callback)
        {
            if (callback.IsNull())
            {
                return;
            }
            sortOrderCallback = callback;
        }

        protected void InvokeSortOrderCallback(bool isRefresh = true)
        {
            if (!sortOrderCallback.IsNull())
            {
                sortOrderCallback(isRefresh);
            }
        }

        public void SetSortOrder(string propName, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            if (propName.IsNullOrEmpty() || this.ContentView.IsNull())
            {
                return;
            }

            this.ClearSortOptions();

            if (sortDirection == ListSortDirection.Ascending)
            {
                this.ContentView.SortDescriptions.Add(new SortDescription(propName, sortDirection));
            }
            else
            {
                this.ContentView.SortDescriptions.Add(new SortDescription(propName, sortDirection));
            }
        }

        public void ClearSortOptions()
        {
            if (!this.ContentView.IsNull() && this.ContentView.SortDescriptions.Count > 0)
            {
                RunOnUIThread(() =>
                {
                    this.ContentView.SortDescriptions.Clear();
                });
            }
        }

        #endregion

        protected override void OnDisposing(bool isDisposing)
        {
            this.sortOrderCallback = null;
            this.ClearSortOptions();
            base.OnDisposing(isDisposing);
        }
    }
}
