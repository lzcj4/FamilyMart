using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace FamilyMartUI.Common
{
    public class OverRangeWarnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush result = Brushes.Black;
            if (value == null||parameter==null)
                return result;

            double maxValue = System.Convert.ToDouble(parameter);
            double dValue = (double)value;
            if (dValue >= maxValue)
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
