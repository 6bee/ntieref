using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace BlogWriter.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType=typeof(bool))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public const bool True = true;
        public const bool Invert = True;

        public BooleanToVisibilityConverter()
        {
            InvisibleState = Visibility.Collapsed;
        }

        public Visibility InvisibleState { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inverse = parameter is bool && (bool)parameter;
            return (value != null && (bool)value) ^ inverse ? Visibility.Visible : InvisibleState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}