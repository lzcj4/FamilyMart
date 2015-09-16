using System.IO;
using System.Windows;
using DAL;
using DAL.Model;

namespace FamilyMartUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TestParse();
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
        }
    }
}

