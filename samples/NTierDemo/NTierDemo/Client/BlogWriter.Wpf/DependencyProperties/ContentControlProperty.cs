using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlogWriter.Wpf.DependencyProperties
{
    public static class ContentControlProperty
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
            "IsActive",
            typeof(bool),
            typeof(ContentControlProperty),
            new PropertyMetadata(true));

        public static void SetIsActive(this ContentControl control, bool value)
        {
            control.SetValue(IsActiveProperty, value);
        }

        public static bool GetIsActive(this ContentControl control)
        {
            return (bool)control.GetValue(IsActiveProperty);
        }
    }
}
