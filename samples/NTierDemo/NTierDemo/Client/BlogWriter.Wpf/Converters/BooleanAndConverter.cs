using System;
using System.Linq;
using System.Windows.Data;

namespace BlogWriter.Wpf.Converters
{
    public sealed class BooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values != null && values.All(i => i is bool && (bool)i);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
