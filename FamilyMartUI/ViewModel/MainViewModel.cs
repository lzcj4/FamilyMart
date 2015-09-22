using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DAL.Common;
using DAL.Model;
using FamilyMartUI.Common;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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

        public void LoadAsync(Action callback)
        {
            Action action = () =>
            {
                var list = FMDBHelper.Instance.GetDialyReport();
                Add(list, callback);
            };

            action.BeginInvoke((ar) => { action.EndInvoke(ar); }, this);
        }

        public void LoadAsync(string startDate, string endDate, Action callback)
        {
            Action action = () =>
            {
                var list = FMDBHelper.Instance.GetDialyReport(startDate, endDate);
                Add(list, callback);
            };

            action.BeginInvoke((ar) => { action.EndInvoke(ar); }, this);
        }

        private void Add(IEnumerable<DialyReport> list, Action callback)
        {
            this.RunOnUIThreadAsync(() =>
                          {
                              this.DialyViewModel.Items.Clear();
                              foreach (var item in list)
                              {
                                  this.DialyViewModel.Items.Add(item);
                              }
                              UpdateStatisticSummary();
                              if (callback != null)
                              {
                                  callback();
                              }
                          });
        }


        void UpdateStatisticSummary()
        {
            StringBuilder sb = new StringBuilder();
            var list = this.DialyViewModel.Items;
            if (list.IsNullOrEmpty())
            {
                this.DialyViewModel.SummaryText = sb.ToString();
                return;
            }

            var workDayList = list.Where(item => !item.IsWeekend);
            var weekendList = list.Where(item => item.IsWeekend);
            int paddingLen = 50;
            char paddingChar = ' ';

            string amount = string.Format("总日销:{0},工作日销:{1},周末日销:{2};",
                                    list.Sum(item => item.Amount), workDayList.Sum(item => item.Amount),
                                    weekendList.Sum(item => item.Amount)).PadRight(paddingLen, paddingChar);
            sb.Append(amount);

            string waste = string.Format("\t总损耗:{0},工作日损耗:{1},周末损耗:{2};",
                                      list.Sum(item => item.Waste), workDayList.Sum(item => item.Waste),
                                      weekendList.Sum(item => item.Waste));
            sb.AppendLine(waste);

            string parttime = string.Format("总兼职:{0},工作日兼职:{1},周末兼职:{2};",
                                            list.Sum(item => item.ParttimeEmployee), workDayList.Sum(item => item.ParttimeEmployee),
                                            weekendList.Sum(item => item.ParttimeEmployee)).PadRight(paddingLen, paddingChar);
            sb.Append(parttime);

            string employee = string.Format("\t总正职:{0},工作日正职:{1},周末正职:{2};",
                                          list.Sum(item => item.Employee), workDayList.Sum(item => item.Employee),
                                          weekendList.Sum(item => item.Employee));
            sb.AppendLine(employee);

            string electric = string.Format("总电表:{0},工作日电表:{1},周末电表:{2};",
                                    list.Sum(item => item.ElectrictCharge), workDayList.Sum(item => item.ElectrictCharge),
                                    weekendList.Sum(item => item.ElectrictCharge)).PadRight(paddingLen, paddingChar);
            sb.Append(electric);
            this.DialyViewModel.SummaryText = sb.ToString();
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
