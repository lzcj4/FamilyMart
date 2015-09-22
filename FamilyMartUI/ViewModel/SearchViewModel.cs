using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using FamilyMartUI.Common;
using FamilyMartUI.UC;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace FamilyMartUI.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        const string dateFormat = "yyyy-MM-dd";

        private string startDate = DateTime.Now.Date.AddDays(-7).ToString(dateFormat);
        public string StartDate
        {
            get { return startDate; }
            set
            {
                SetProperty(ref startDate, FormatDate(value), "StartDate");
            }
        }

        private string endDate = DateTime.Now.Date.ToString(dateFormat);
        public string EndDate
        {
            get { return endDate; }
            set
            {
                SetProperty(ref endDate, FormatDate(value), "EndDate");
            }
        }

        private ICollectionView goodsTypeView;
        public ICollectionView GoodsTypeView
        {
            get { return goodsTypeView; }
            set
            {
                SetProperty(ref goodsTypeView, value, "GoodsTypeView");
            }
        }

        private StatisticType currentStatisticType = StatisticType.Amount;
        public StatisticType CurrentStatisticType
        {
            get { return currentStatisticType; }
            set
            {
                if (currentStatisticType != value)
                {
                    currentStatisticType = value;
                    RaiseOnSearch(false);
                }
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new GenericCommand()
                {
                    ExecuteCallback = (obj) =>
                    {
                        RaiseOnSearch(true);
                    },
                    CanExecuteCallback = (obj) => { return true; }
                };
            }
        }

        public ICommand DetailCommand
        {
            get
            {
                return new GenericCommand()
                {
                    ExecuteCallback = (obj) =>
                    {
                        RaiseOnSearch(null);
                    },
                    CanExecuteCallback = (obj) => { return true; }
                };
            }
        }

        public event EventHandler<EventArgs<SearchViewModel, bool?>> OnSearch;
        private void RaiseOnSearch(bool? isReload)
        {
            if (null != OnSearch)
            {
                OnSearch(this, new EventArgs<SearchViewModel, bool?>(this, isReload));
            }
        }

        public SearchViewModel()
        {
            this.LoadGoodsTypes();
            this.GoodsTypeView = CollectionViewSource.GetDefaultView(this.goodsTypeList);
        }


        private ObservableCollection<ItemModel> goodsTypeList = new ObservableCollection<ItemModel>();
        private void LoadGoodsTypes()
        {
            goodsTypeList.Add(new ItemModel("营业额", StatisticType.Amount));
            goodsTypeList.Add(new ItemModel("到客数", StatisticType.Customer));
            goodsTypeList.Add(new ItemModel("报废", StatisticType.Waste));
            goodsTypeList.Add(new ItemModel("工时", StatisticType.WorkHours));
            goodsTypeList.Add(new ItemModel("电费", StatisticType.Electric));
            goodsTypeList.Add(new ItemModel("盒饭", StatisticType.BoxLaunch));
            goodsTypeList.Add(new ItemModel("面包", StatisticType.Bread));
        }
        private string FormatDate(string date)
        {
            string[] parts = date.Split(new char[] { '/' });
            if (parts.Length == 3)
            {
                return string.Format("{0:D4}-{1:D2}-{2:D2}", Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]));
            }
            else
            {
                throw new InvalidOperationException();

            }
        }
    }

    public class ItemModel
    {
        public string Title { get; set; }
        public StatisticType Value { get; set; }
        public ItemModel(string title, StatisticType statisticValue)
        {
            this.Title = title;
            this.Value = statisticValue;
        }
    }

    public enum StatisticType
    {
        Amount = 0,
        Customer = 1,
        Waste = 2,
        WorkHours = 3,
        Electric = 4,
        BoxLaunch = 5,
        Bread = 6,
        Noodel = 7,
        RiceRoll = 8,
        Sushi = 9,

    }
}
