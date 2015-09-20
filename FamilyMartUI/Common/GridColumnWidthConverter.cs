using System;
using System.Windows.Data;
using System.Windows.Media;

namespace FamilyMartUI.Common
{
    public class GridColumnWidthConverter : IValueConverter
    {
        const int offset = 30;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;

            double width = 0;
            double.TryParse(value.ToString(), out width);
            if (width < offset)
                return 0;

            return width - offset;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DayOfWeekForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is DateTime))
                return Brushes.Black;

            DateTime date = (DateTime)value;
            Brush result = Brushes.Black;
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                result = Brushes.Red;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
