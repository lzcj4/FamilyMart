using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DAL.Common;
using DAL.Model;
using FamilyMartUI.Common;

namespace FamilyMartUI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        SortOrderViewModel<DialyReport> dialyViewModel = new SortOrderViewModel<DialyReport>();
        public SortOrderViewModel<DialyReport> DialyViewModel
        {
            get { return dialyViewModel; }
        }

        SortOrderViewModel<GoodsRecord> detailViewModel = new SortOrderViewModel<GoodsRecord>();
        public SortOrderViewModel<GoodsRecord> DetailViewModel
        {
            get { return detailViewModel; }
        }

        SearchViewModel queryViewModel = new SearchViewModel();
        public SearchViewModel SearchViewModel
        {
            get { return queryViewModel; }
        }

        #endregion

        public MainViewModel()
        {
        }

        public void LoadAsync(Action callback)
        {
            Action action = () =>
            {
                var list = FMDBHelper.Instance.GetDialyReport();
                this.RunOnUIThreadAsync(() =>
                {
                    this.DialyViewModel.Items.Clear();
                    foreach (var item in list)
                    {
                        this.DialyViewModel.Items.Add(item);
                    }
                    this.DialyViewModel.ContentView.MoveCurrentToFirst();
                    if (callback != null)
                    {
                        callback();
                    }
                });
            };

            action.BeginInvoke((ar) => { action.EndInvoke(ar); }, this);
        }

        public void LoadAsync(string startDate, string endDate, Action callback)
        {
            Action action = () =>
            {
                var list = FMDBHelper.Instance.GetDialyReport(startDate, endDate);
                this.RunOnUIThreadAsync(() =>
                {
                    this.DialyViewModel.Items.Clear();
                    foreach (var item in list)
                    {
                        this.DialyViewModel.Items.Add(item);
                    }
                    if (callback != null)
                    {
                        callback();
                    }
                });
            };

            action.BeginInvoke((ar) => { action.EndInvoke(ar); }, this);
        }

        public void LoadDetail(DialyReport report)
        {
            this.RunOnUIThreadAsync(() =>
            {
                this.DetailViewModel.Items.Clear();
                foreach (var item in report.Details)
                {
                    this.DetailViewModel.Items.Add(item);
                }
            });
        }

        protected override void OnDisposing(bool isDisposing)
        {
            try
            {
                Logger.Info("---FileExplorerViewModel disposing----");
                base.OnDisposing(isDisposing);
            }
            catch (Exception)
            {
            }
        }
    }
}
