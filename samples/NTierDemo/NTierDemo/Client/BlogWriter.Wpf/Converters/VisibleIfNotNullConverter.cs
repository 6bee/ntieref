using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace BlogWriter.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType=typeof(Visibility))]
    public sealed class VisibleIfNotNullConverter : VisibleIfNullConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, true, parameter);
        }
    }
}