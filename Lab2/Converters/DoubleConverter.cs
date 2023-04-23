using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SpringLab2.Converters
{
    class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "" : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? strvalue = value as string;
            if (Double.TryParse(strvalue, out double val))
            {
                return val;
            }
            else
            {
                MessageBox.Show("Your input is not numeric");
                return 0;
            }
        }
    }
}
