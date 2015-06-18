using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace BlogWriter.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType=typeof(Visibility))]
    public class VisibleIfNullConverter : IValueConverter
    {
        public VisibleIfNullConverter()
        {
            InvisibleState = Visibility.Collapsed;
        }

        public Visibility InvisibleState { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, false, parameter);
        }

        protected object Convert(object value, bool inverse, object parameter)
        {
            var invisibleState = InvisibleState;

            Visibility v;
            if (parameter is Visibility)
            {
                invisibleState = (Visibility)parameter;
            }
            else if (parameter is string && Enum.TryParse<Visibility>(parameter.ToString(), out v))
            {
                invisibleState = v;
            }
         
            return ReferenceEquals(null, value) ^ inverse ? Visibility.Visible : invisibleState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}