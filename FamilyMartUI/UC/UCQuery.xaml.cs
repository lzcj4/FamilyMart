using System.Windows.Controls;
using FamilyMartUI.ViewModel;

namespace FamilyMartUI.UC
{
    /// <summary>
    /// Interaction logic for UCQuery.xaml
    /// </summary>
    public partial class UCQuery : UserControl
    {
        public UCQuery()
        {
            InitializeComponent();
        }

        QueryViewModel ViewModel
        {
            get { return this.DataContext as QueryViewModel; }
        }

        private void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            RadioButton radioBtn = sender as RadioButton;
            if (radioBtn == null)
            {
                return;
            }
            string content = radioBtn.Content.ToString();
            switch (content)
            {
                case "日销":
                    this.ViewModel.CurrentStatisticType = StatisticType.Amount;
                    break;
                case "来客":
                    this.ViewModel.CurrentStatisticType = StatisticType.Customer;
                    break;
                case "损耗":
                    this.ViewModel.CurrentStatisticType = StatisticType.Waste;
                    break;
                case "工时":
                    this.ViewModel.CurrentStatisticType = StatisticType.WorkHours;
                    break;
                case "电表":
                    this.ViewModel.CurrentStatisticType = StatisticType.Electric;
                    break;
                case "盒饭":
                    this.ViewModel.CurrentStatisticType = StatisticType.BoxLaunch;
                    break;
                case "面包":
                    this.ViewModel.CurrentStatisticType = StatisticType.Bread;
                    break;
                case "调理面":
                    this.ViewModel.CurrentStatisticType = StatisticType.Noodel;
                    break;
                case "饭团":
                    this.ViewModel.CurrentStatisticType = StatisticType.RiceRoll;
                    break;
                case "寿司":
                    this.ViewModel.CurrentStatisticType = StatisticType.Sushi;
                    break;
                default:
                    break;
            }

        }
    }
}
