using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FamilyMartUI.ViewModel;
using FamilyMartUI.Properties;
using FamilyMartUI.UC;

namespace FamilyMartUI
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        public ChartWindow()
        {
            InitializeComponent();
            this.Loaded += ChartWindow_Loaded;
        }

        void ChartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            chkAmount.IsChecked = true;
            chkCustomer.IsChecked = true;
            chkWaste.IsChecked = true;
            chkWorkHours.IsChecked = true;
            chkElectric.IsChecked = true;
            chkBoxLaunch.IsChecked = true;
            chkBread.IsChecked = true;
            chkNoodle.IsChecked = true;
            chkRiceRoll.IsChecked = true;
            chkSushi.IsChecked = true;
            chkJiXiang.IsChecked = true;
        }

        MainViewModel ViewModel
        {
            get { return this.DataContext as MainViewModel; }
        }

        StatisticType CurrentStatisticType = StatisticType.Amount;

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null || this.ViewModel == null)
            {
                return;
            }
            OnCheckedChanged(chkBox);
        }

        private void CheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox == null || this.ViewModel == null)
            {
                return;
            }
            OnCheckedChanged(chkBox);
        }
        private void OnCheckedChanged(CheckBox chkBox)
        {
            if (chkBox == null || this.ViewModel == null)
            {
                return;
            }
            string content = chkBox.Content.ToString();
            switch (content)
            {
                case "日销":
                    this.CurrentStatisticType = StatisticType.Amount;
                    break;
                case "来客":
                    this.CurrentStatisticType = StatisticType.Customer;
                    break;
                case "损耗":
                    this.CurrentStatisticType = StatisticType.Waste;
                    break;
                case "工时":
                    this.CurrentStatisticType = StatisticType.WorkHours;
                    break;
                case "电表":
                    this.CurrentStatisticType = StatisticType.Electric;
                    break;
                case "盒饭":
                    this.CurrentStatisticType = StatisticType.BoxLaunch;
                    break;
                case "面包":
                    this.CurrentStatisticType = StatisticType.Bread;
                    break;
                case "调理面":
                    this.CurrentStatisticType = StatisticType.Noodel;
                    break;
                case "饭团":
                    this.CurrentStatisticType = StatisticType.RiceRoll;
                    break;
                case "寿司":
                    this.CurrentStatisticType = StatisticType.Sushi;
                    break;
                case "集享卡":
                    this.CurrentStatisticType = StatisticType.JiXiang;
                    break;
                default:
                    break;
            }
            if (chkBox.IsChecked==true) {
                UpadteStatisticChart();
            }
            else
            {
                RemoveStatisticChart();
            }
        }

        void RemoveStatisticChart()
        {
            DateTime[] datetimes = new DateTime[0];
            int[] levels = new int[0];
            double[][] datas = new double[0][] ;
            string[] emptyStrings = new string[0];

            switch (this.CurrentStatisticType)
            {
                case StatisticType.Amount:
                    ucAmount.SetData(datetimes, levels, datas, "日销", emptyStrings, oneBrush);
                    break;
                case StatisticType.Customer:
                    ucCustomer.SetData(datetimes, levels, datas, "来客", emptyStrings, oneBrush);
                    break;
                case StatisticType.Waste:
                    ucWaste.SetData(datetimes, levels, datas, "损耗", emptyStrings, oneBrush);
                    break;
                case StatisticType.WorkHours:
                    ucWorkHours.SetData(datetimes, levels, datas, "工时", emptyStrings, oneBrush);
                    break;
                case StatisticType.Electric:
                    ucElectric.SetData(datetimes, levels, datas, "电表", emptyStrings, oneBrush);
                    break;
                case StatisticType.BoxLaunch:
                    ucBoxLaunch.SetData(datetimes, levels, datas, "盒饭", threeTitles, threeBrushes);
                    break;
                case StatisticType.Bread:
                    ucBread.SetData(datetimes, levels, datas, "面包", threeTitles, threeBrushes);
                    break;
                case StatisticType.Noodel:
                    ucNoodle.SetData(datetimes, levels, datas, "调理面", threeTitles, threeBrushes);
                    break;
                case StatisticType.RiceRoll:
                    ucRiceRoll.SetData(datetimes, levels, datas, "饭团", threeTitles, threeBrushes);
                    break;
                case StatisticType.Sushi:
                    ucSuShi.SetData(datetimes, levels, datas, "寿司", threeTitles, threeBrushes);
                    break;
                case StatisticType.JiXiang:
                    ucJiXiang.SetData(datetimes, levels, datas, "集享卡", threeTitles, threeBrushes);
                    break;
                default:
                    break;
            }
        }

        Brush[] oneBrush = new Brush[] { Brushes.Blue };
        Brush[] threeBrushes = new Brush[] { Brushes.Green, Brushes.Blue, Brushes.Red };
        string[] threeTitles = new string[] { "进", "销", "废" };

        void UpadteStatisticChart()
        {
            var list = this.ViewModel.DialyViewModel.Items;
            DateTime[] datetimes = list.Select(item => item.SaleDate).ToArray();
            int[] levels = new int[10];
            double[][] datas;
            string[] emptyStrings = new string[0];

            switch (this.CurrentStatisticType)
            {
                case StatisticType.Amount:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Amount;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.Amount).ToArray();
                    ucAmount.SetData(datetimes, levels, datas, "日销", emptyStrings, oneBrush);
                    break;
                case StatisticType.Customer:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Customer;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => (double)item.Customer).ToArray();
                    ucCustomer.SetData(datetimes, levels, datas, "来客", emptyStrings, oneBrush);
                    break;
                case StatisticType.Waste:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Waste;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.Waste).ToArray();
                    ucWaste.SetData(datetimes, levels, datas, "损耗", emptyStrings, oneBrush);
                    break;
                case StatisticType.WorkHours:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.WorkHours;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.ParttimeEmployee + item.Employee).ToArray();
                    ucWorkHours.SetData(datetimes, levels, datas, "工时", emptyStrings, oneBrush);
                    break;
                case StatisticType.Electric:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Electric;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.ElectrictCharge).ToArray();
                    ucElectric.SetData(datetimes, levels, datas, "电表", emptyStrings, oneBrush);
                    break;
                case StatisticType.BoxLaunch:
                    SetThreeLineByType(ucBoxLaunch, datetimes, Settings.Default.BoxLaunch, "盒饭");
                    break;
                case StatisticType.Bread:
                    SetThreeLineByType(ucBread, datetimes, Settings.Default.Bread, "面包");
                    break;
                case StatisticType.Noodel:
                    SetThreeLineByType(ucNoodle, datetimes, Settings.Default.Noodel, "调理面");
                    break;
                case StatisticType.RiceRoll:
                    SetThreeLineByType(ucRiceRoll, datetimes, Settings.Default.RiceRoll, "饭团");
                    break;
                case StatisticType.Sushi:
                    SetThreeLineByType(ucSuShi, datetimes, Settings.Default.SuShi, "寿司");
                    break;
                case StatisticType.JiXiang:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.JiXiang;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.Details.FirstOrDefault(subItem => subItem.Goods.Name == "集享卡"))
                                                                 .Select(item => item.FirstSale).ToArray();
                    ucJiXiang.SetData(datetimes, levels, datas, "集享卡", emptyStrings, oneBrush);
                    break;
                default:
                    break;
            }
        }

        void SetThreeLineByType(UCChart ucChart, DateTime[] datetimes, int step, string typeName)
        {
            var list = this.ViewModel.DialyViewModel.Items;
            int[] levels = new int[10];
            for (int i = 0; i < 10; i++)
            {
                levels[i] = (i + 1) * step;
            }

            double[][] datas = new double[3][];
            var subList = list.Select(item => item.Details.FirstOrDefault(subItem => subItem.Goods.Name == typeName));
            datas[0] = subList.Select(item => item.FirstIn).ToArray();
            datas[1] = subList.Select(item => item.FirstSale).ToArray();
            datas[2] = subList.Select(item => item.FirstWaste).ToArray();
            ucChart.SetData(datetimes, levels, datas, typeName, threeTitles, threeBrushes);
        }
    }
}
