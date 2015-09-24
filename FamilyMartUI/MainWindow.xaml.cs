using System.IO;
using System.Windows;
using DAL;
using DAL.Model;
using FamilyMartUI.ViewModel;
using System.Linq;
using System;
using System.Windows.Media;
using FamilyMartUI.Properties;
using System.Text;
using FamilyMartUI.Common;
using System.ComponentModel;

namespace FamilyMartUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel
        {
            get { return this.DataContext as MainViewModel; }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.ucDialyReport.OnSelectionChanged += ucDialyReport_OnSelectionChanged;
            this.ucParser.OnChanged += ucParser_OnChanged;
            this.ViewModel.SearchViewModel.OnSearch += SearchViewModel_OnSearch;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.LoadAsync(UpadteStatisticChart);
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            FMDBHelper.Instance.Dispose();
        }

        void ucDialyReport_OnSelectionChanged(object sender, UC.EventArgs<DialyReport> e)
        {
            this.ViewModel.LoadDetail(e.Content);
        }

        void ucParser_OnChanged(object sender, System.EventArgs e)
        {
            this.ViewModel.LoadAsync(UpadteStatisticChart);
        }

        void SearchViewModel_OnSearch(object sender, UC.EventArgs<SearchViewModel, bool?> e)
        {
            if (e.Item2 == null)
            {
                ChartWindow chartWindow = new ChartWindow();
                chartWindow.DataContext = this.DataContext;
                chartWindow.Show();
            }
            else if (e.Item2 == true)
            {
                this.ViewModel.LoadAsync(e.Item1.StartDate, e.Item1.EndDate, UpadteStatisticChart);
            }
            else if (e.Item2 == false)
            {
                UpadteStatisticChart();
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

            switch (this.ViewModel.SearchViewModel.CurrentStatisticType)
            {
                case StatisticType.Amount:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Amount;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.Amount).ToArray();
                    ucChart.SetData(datetimes, levels, datas, "日销", emptyStrings, oneBrush);
                    break;
                case StatisticType.Customer:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Customer;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => (double)item.Customer).ToArray();
                    ucChart.SetData(datetimes, levels, datas, "来客", emptyStrings, oneBrush);
                    break;
                case StatisticType.Waste:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Waste;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.Waste).ToArray();
                    ucChart.SetData(datetimes, levels, datas, "损耗", emptyStrings, oneBrush);
                    break;
                case StatisticType.WorkHours:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.WorkHours;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.ParttimeEmployee + item.Employee).ToArray();
                    ucChart.SetData(datetimes, levels, datas, "工时", emptyStrings, oneBrush);
                    break;
                case StatisticType.Electric:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Electric;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.ElectrictCharge).ToArray();
                    ucChart.SetData(datetimes, levels, datas, "电表", emptyStrings, oneBrush);
                    break;
                case StatisticType.BoxLaunch:
                    SetThreeLineByType(datetimes, Settings.Default.BoxLaunch, "盒饭");
                    break;
                case StatisticType.Bread:
                    SetThreeLineByType(datetimes, Settings.Default.Bread, "面包");
                    break;
                case StatisticType.Noodel:
                    SetThreeLineByType(datetimes, Settings.Default.Noodel, "调理面");
                    break;
                case StatisticType.RiceRoll:
                    SetThreeLineByType(datetimes, Settings.Default.RiceRoll, "饭团");
                    break;
                case StatisticType.Sushi:
                    SetThreeLineByType(datetimes, Settings.Default.SuShi, "寿司");
                    break;
                case StatisticType.JiXiang:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.JiXiang;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.Details.FirstOrDefault(subItem => subItem.Goods.Name == "集享卡"))
                                                                 .Select(item => item.FirstSale).ToArray();
                    ucChart.SetData(datetimes, levels, datas, "集享卡", emptyStrings, oneBrush);
                    break;
                default:
                    break;
            }
        }

        void SetThreeLineByType(DateTime[] datetimes, int step, string typeName)
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

