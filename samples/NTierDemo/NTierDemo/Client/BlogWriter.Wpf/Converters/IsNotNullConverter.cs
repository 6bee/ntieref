using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace BlogWriter.Wpf.Converters
{
    [ValueConversion(typeof(object), typeof(bool), ParameterType=typeof(bool))]
    public sealed class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !ReferenceEquals(value, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}