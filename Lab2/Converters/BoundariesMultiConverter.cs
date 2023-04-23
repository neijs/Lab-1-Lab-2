using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SpringLab2.Converters
{
    public class BoundariesMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values
          .Cast<double>()
          .Aggregate(
            string.Empty,
            (newStringValue, doubleValue) => $"{newStringValue};{doubleValue}",
            result => result.Trim(';'));

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            try
            {
               return ((string)value)
              .Split(new[] { ';', ' ', '/' }, StringSplitOptions.RemoveEmptyEntries)
              .Select(textValue => (object)double.Parse(textValue))
              .ToArray();
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
                object[] result = new object[2] { (double)0, (double)1};
                return result;
            }
        }
    }
}
