using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DAL;
using DAL.Model;
using FamilyMartUI.Common;
using Microsoft.Win32;
using System.Text;

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
        private void ButtonText_Click(object sender, RoutedEventArgs e)
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

        private void ButtonFile_Click(object sender, RoutedEventArgs e)
        {
            FMDBHelper dbHelper = FMDBHelper.Instance;
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Multiselect = true;
            openDlg.ShowDialog();
            string[] filePaths = openDlg.FileNames;
            bool isExisted = false;

            Action action = new Action(() =>
            {
                //Directory.GetFiles(@"E:\FM Record", "*");
                StringBuilder sb = new StringBuilder();
                foreach (string item in filePaths)
                {
                    using (FileStream fs = new FileStream(item, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string s = sr.ReadToEnd();
                            try
                            {
                                var v = DialyReportParser.Parse(s);
                                if (v != null)
                                {
                                    dbHelper.InsertDialyReport(v);
                                    isExisted = true;
                                }
                            }
                            catch (System.Exception ex)
                            {                            	    
                            }                        

                            string error = DialyReportParser.GetLatestError();
                            if (!error.IsNullOrEmpty())
                            {
                                sb.AppendLine(error);
                            }
                        }
                    }
                }

                this.Dispatcher.BeginInvoke((Delegate)new Action(() =>
                {
                    if (sb.Length > 0)
                    {
                        txtReport.Text = sb.ToString();
                    }
                    if (isExisted && OnChanged != null)
                    {
                        OnChanged(this, new EventArgs());
                    }
                }));

            });
            action.BeginInvoke((ar) => { action.EndInvoke(ar); }, action);
        }
    }
}
