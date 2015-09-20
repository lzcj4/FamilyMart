﻿using System.IO;
using System.Windows;
using DAL;
using DAL.Model;
using FamilyMartUI.ViewModel;
using System.Linq;
using System;

namespace FamilyMartUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReportViewModel ViewModel
        {
            get { return this.DataContext as ReportViewModel; }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new ReportViewModel();

            this.Loaded += MainWindow_Loaded;
            this.ucDialyReport.OnSelectionChanged += ucDialyReport_OnSelectionChanged;
            this.ucParser.OnChanged += ucParser_OnChanged;
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

        void UpadteStatisticChart()
        {
            var list = this.ViewModel.DialyViewModel.Items;
           DateTime[] datetimes = list.Select(item => item.SaleDate).ToArray();
            int[] levels = new int[10];
            for (int i = 0; i < 10; i++)
            {
                levels[i] = (i + 1) * 30;
            }

            double[][] datas = new double[3][];

            datas[0] = list.Select(item => item.Details.FirstOrDefault(subItem => subItem.Goods.Name == "盒饭").FirstIn).ToArray();
            datas[1] = list.Select(item => item.Details.FirstOrDefault(subItem => subItem.Goods.Name == "盒饭").FirstSale).ToArray();
            datas[2] = list.Select(item => item.Details.FirstOrDefault(subItem => subItem.Goods.Name == "盒饭").FirstWaste).ToArray();

            ucChart.SetXYAxisAndData(datetimes,levels,datas);
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
