using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ProductManager.Common.Domain.Model.ProductManager;
using ProductManager.WPF.Applications.ViewModels;
using ProductManager.WPF.Presentation.Properties;

namespace ProductManager.WPF.Presentation.Converters
{
    public class ProductColorToStringConverter : IValueConverter
    {
        private static readonly ProductColorToStringConverter defaultInstance = new ProductColorToStringConverter();

        public static ProductColorToStringConverter Default { get { return defaultInstance; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ProductColorFilter)) { return null; }

            ProductColorFilter sizeUnit = (ProductColorFilter)value;
            switch (sizeUnit)
            {
                case ProductColorFilter.NoFilter:
                    return Resources.UndefinedEmpty;
                default:
                    return sizeUnit.ToString();
            }
            throw new InvalidOperationException("Enum value is unknown.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
