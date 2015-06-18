using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ProductManager.WPF.Presentation.Converters
{
    public class PositiveDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var number = (decimal?)value;
            return number.HasValue ? number.Value.ToString() : string.Empty;
        }

        // convert invalid input to negative value enforcing validation to fail
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double number;
            if (double.TryParse(value as string, out number))
            {
                return number;
            }
            return -1m;
        }
    }
}
