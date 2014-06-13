using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlogWriter.Wpf.DependencyProperties
{
    public static class ContentControl
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
            "IsActive",
            typeof(bool),
            typeof(ContentControl),
            new PropertyMetadata(true));

        public static void SetIsActive(this System.Windows.Controls.ContentControl control, bool value)
        {
            control.SetValue(IsActiveProperty, value);
        }

        public static bool GetIsActive(this System.Windows.Controls.ContentControl control)
        {
            return (bool)control.GetValue(IsActiveProperty);
        }
    }
}
