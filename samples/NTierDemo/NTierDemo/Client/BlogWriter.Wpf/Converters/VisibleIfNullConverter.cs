using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace BlogWriter.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType=typeof(bool))]
    public class VisibleIfNullConverter : IValueConverter
    {
        public const bool True = true;
        public const bool Invert = True;

        public VisibleIfNullConverter()
        {
            InvisibleState = Visibility.Collapsed;
        }

        public Visibility InvisibleState { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inverse = parameter is bool && (bool)parameter;
            return ReferenceEquals(null, value) ^ inverse ? Visibility.Visible : InvisibleState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}