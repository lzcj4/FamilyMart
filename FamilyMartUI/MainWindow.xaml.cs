using System.IO;
using System.Windows;
using DAL;
using DAL.Model;
using FamilyMartUI.ViewModel;
using System.Linq;
using System;
using System.Windows.Media;
using FamilyMartUI.Properties;

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
            this.ucDialyReport.OnSelectionChanged += ucDialyReport_OnSelectionChanged;
            this.ucParser.OnChanged += ucParser_OnChanged;
            this.ViewModel.QueryViewModel.OnQuery += QueryViewModel_OnQuery;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.LoadAsync(UpadteStatisticChart);
        }

        void ucDialyReport_OnSelectionChanged(object sender, UC.EventArgs<DialyReport> e)
        {
            this.ViewModel.LoadDetail(e.Content);
        }

        void ucParser_OnChanged(object sender, System.EventArgs e)
        {
            this.ViewModel.LoadAsync(UpadteStatisticChart);
        }

        void QueryViewModel_OnQuery(object sender, UC.EventArgs<QueryViewModel, bool> e)
        {
            if (e.Item2)
            {
                this.ViewModel.LoadAsync(e.Item1.StartDate, e.Item1.EndDate, UpadteStatisticChart);
            }
            else
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

            switch (this.ViewModel.QueryViewModel.CurrentStatisticType)
            {
                case StatisticType.Amount:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Amount;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.Amount).ToArray();
                    ucChart.SetTitleAndBrushes(new string[] { "日销" }, oneBrush);
                    ucChart.SetXYAxisAndData(datetimes, levels, datas);
                    break;
                case StatisticType.Customer:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Customer;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => (double)item.Customer).ToArray();
                    ucChart.SetTitleAndBrushes(new string[] { "来客" }, oneBrush);
                    ucChart.SetXYAxisAndData(datetimes, levels, datas);
                    break;
                case StatisticType.Waste:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Waste;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.Waste).ToArray();
                    ucChart.SetTitleAndBrushes(new string[] { "损耗" }, oneBrush);
                    ucChart.SetXYAxisAndData(datetimes, levels, datas);
                    break;
                case StatisticType.WorkHours:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.WorkHours;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.ParttimeEmployee + item.Employee).ToArray();
                    ucChart.SetTitleAndBrushes(new string[] { "工时" }, oneBrush);
                    ucChart.SetXYAxisAndData(datetimes, levels, datas);
                    break;
                case StatisticType.Electric:
                    for (int i = 0; i < 10; i++)
                    {
                        levels[i] = (i + 1) * Settings.Default.Electric;
                    }

                    datas = new double[1][];
                    datas[0] = list.Select(item => item.ElectrictCharge).ToArray();
                    ucChart.SetTitleAndBrushes(new string[] { "电表" }, oneBrush);
                    ucChart.SetXYAxisAndData(datetimes, levels, datas);
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
                default:
                    break;
            }

        }

        private void SetThreeLineByType(DateTime[] datetimes, int step, string typeName)
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
            ucChart.SetTitleAndBrushes(threeTitles, threeBrushes);
            ucChart.SetXYAxisAndData(datetimes, levels, datas);
        }

        private void TestParse()
        {
            FMDBHelper dbHelper = FMDBHelper.Instance;
            string[] filePaths = Directory.GetFiles(@"E:\FM Record", "*");
            foreach (string item in filePaths)
            {
                using (FileStream fs = new FileStream(item, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string s = sr.ReadToEnd();
                        var v = DialyReportParser.Parse(s);
                        if (v != null)
                        {
                            dbHelper.InsertDialyReport(v);
                        }
                    }
                }
            }
            //MessageBox.Show(string.Format("成功导入:{0}个文件数据", filePaths.Length));
            //this.ViewModel.LoadAsync();
        }
    }
}

