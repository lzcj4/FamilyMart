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
using System.ComponentModel;
using System.Collections.ObjectModel;
using DAL.Model;
using FamilyMartUI.Common;
using DAL;

namespace FamilyMartUI
{
    /// <summary>
    /// Interaction logic for GoodsWindows.xaml
    /// </summary>
    public partial class GoodsWindows : Window
    {
        #region DP

        /// <summary>
        ///Content view
        /// </summary>
        public static readonly DependencyProperty ContentViewProperty =
            DependencyProperty.Register("ContentView", typeof(ICollectionView), typeof(GoodsWindows));

        public static void SetContentView(DependencyObject element, ICollectionView value)
        {
            if (element == null) return;
            element.SetValue(ContentViewProperty, value);
        }

        public static ICollectionView GetContentView(DependencyObject element)
        {
            if (element == null) return null;
            return (ICollectionView)element.GetValue(ContentViewProperty);
        }

        #endregion

        ObservableCollection<Goods> goodsList = new ObservableCollection<Goods>();
        public GoodsWindows()
        {
            InitializeComponent();
            ICollectionView cv = CollectionViewSource.GetDefaultView(goodsList);
            SetContentView(this, cv);
            LoadGoods();
        }

        private void LoadGoods()
        {
            goodsList.Clear();
            var list = FMDBHelper.Instance.GetGoods();
            foreach (var item in list)
            {
                goodsList.Add(item);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string goodsName = txtName.Text.Trim();
            if (goodsName.IsNullOrEmpty())
            {
                MessageBox.Show("名称为空");
                return;
            }
            if (goodsList.Any(item => item.Name == goodsName))
            {
                MessageBox.Show("当前名称已经存在");
                return;
            }
            Goods g = new Goods() { Name = goodsName };
            if (FMDBHelper.Instance.InsertGoods(g))
            {
                LoadGoods();
                DialyReportParser.LoadGoods();
            }
        }
    }
}
