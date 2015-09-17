using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DAL;
using DAL.Model;
using FamilyMartUI.Common;
using Microsoft.Win32;

namespace FamilyMartUI.UC
{
    /// <summary>
    /// Interaction logic for UCReportParser.xaml
    /// </summary>
    public partial class UCReportParser : UserControl
    {
        public UCReportParser()
        {
            InitializeComponent();
        }

        public event EventHandler OnChanged;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string s = txtReport.Text.Trim();
            if (s.IsNullOrEmpty())
            {
                return;
            }

            var item = DialyReportParser.Parse(s);
            if (item != null)
            {
                FMDBHelper.Instance.InsertDialyReport(item);
                if (OnChanged != null)
                {
                    OnChanged(this, new EventArgs());
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FMDBHelper dbHelper = FMDBHelper.Instance;
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Multiselect = true;
            openDlg.ShowDialog();
            string[] filePaths = openDlg.FileNames;
            bool isExisted = false;

            //Directory.GetFiles(@"E:\FM Record", "*");
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
                            isExisted = true;
                        }
                    }
                }
            }

            if (isExisted && OnChanged != null)
            {
                OnChanged(this, new EventArgs());
            }
        }
    }
}
