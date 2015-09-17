﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DAL.Common;
using DAL.Model;
using FamilyMartUI.Common;

namespace FamilyMartUI.ViewModel
{
    public class ReportViewModel : ViewModelBase
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

        #endregion

        public void LoadAsync()
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
