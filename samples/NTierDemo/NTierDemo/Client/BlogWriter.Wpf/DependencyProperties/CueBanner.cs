using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace BlogWriter.Wpf.DependencyProperties
{
    /// <summary>
    /// The static class CueBanner provides a dependency property for ComboBox and TextBox controls 
    /// to allow displaying a cue banner content on these controls by using an Adorner.
    /// </summary>
    /// <remarks>
    /// The cue banner is displayed whenever the control's text property is empty.
    /// </remarks>
    public static class CueBanner
    {
        /// <summary>
        /// Content dependency propertry to allow content binding for cue banner.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
           "Content", typeof(object), typeof(CueBanner),
           new FrameworkPropertyMetadata(null, ContentPropertyChanged));

        /// <summary>
        /// Gets cue banner content property value
        /// </summary>
        /// <param name="control">Control on which the cue banner is adorned</param>
        /// <returns>Cue banner content</returns>
        public static object GetContent(Control control)
        {
            return control.GetValue(ContentProperty);
        }

        /// <summary>
        /// Sets cue banner content property value
        /// </summary>
        /// <param name="control">Control on which the cue banner is adorned</param>
        /// <param name="value">Cue banner content</param>
        public static void SetContent(Control control, object value)
        {
            control.SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Binds to control's event handler to add or remove cue banner according the controls text 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (!(d is ComboBox || d is TextBox)) return;

            var control = (Control)d;
            control.Loaded += (s, e) => RefreshCueBannerVisibility(control);
            control.GotFocus += (s, e) => RefreshCueBannerVisibility(control);
            control.LostFocus += (s, e) => RefreshCueBannerVisibility(control);
            control.IsVisibleChanged += (s, e) => RefreshCueBannerVisibility(control);

            if (d is TextBox)
            {
                ((TextBox)d).TextChanged += (s, e) => RefreshCueBannerVisibility(control);
            }
            if (d is ComboBox)
            {
                ((Selector)d).SelectionChanged += (s, e) =>
                {
                    var c = (Selector)s;
                    if (c.IsVisible && string.IsNullOrWhiteSpace(c.SelectedValue == null ? null : c.SelectedValue.ToString()))
                    {
                        ShowCueBanner(c);
                    }
                    else
                    {
                        RemoveCueBanner(c);
                    }
                };
            }
        }

        /// <summary>
        /// Shows or hides cue banner text depending on the dependency control's state.
        /// </summary>
        /// <param name="c"></param>
        private static void RefreshCueBannerVisibility(Control c)
        {
            if (ShouldShowCueBanner(c))
            {
                ShowCueBanner(c);
            }
            else
            {
                RemoveCueBanner(c);
            }
        }

        private static void RemoveCueBanner(UIElement control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
            if (layer == null) return;

            Adorner[] adorners = layer.GetAdorners(control);
            if (adorners == null) return;

            foreach (Adorner adorner in adorners)
            {
                if (adorner is CueBannerContentAdorner)
                {
                    adorner.Visibility = Visibility.Hidden;
                    layer.Remove(adorner);
                }
            }
        }

        private static void ShowCueBanner(Control control)
        {
            RemoveCueBanner(control);

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
            if (layer == null) return;

            layer.Add(new CueBannerContentAdorner(control, GetContent(control)));
        }

        private static bool ShouldShowCueBanner(Control c)
        {
            if (c.IsKeyboardFocused) return false;
            if (!c.IsVisible) return false;

            DependencyProperty dp = GetDependencyProperty(c);
            return dp == null ? true : c.GetValue(dp).Equals(string.Empty);
        }

        private static DependencyProperty GetDependencyProperty(Control control)
        {
            if (control is ComboBox) return ComboBox.TextProperty;
            if (control is TextBoxBase) return TextBox.TextProperty;
            return null;
        }

        /// <summary>
        /// Adorner implementation to adorn cue banner content on the dependency control
        /// </summary>
        private class CueBannerContentAdorner : Adorner
        {
            private readonly System.Windows.Controls.ContentControl _contentControl;

            public CueBannerContentAdorner(Control adornedElement, object content)
                : base(adornedElement)
            {
                _contentControl = new System.Windows.Controls.ContentControl
                {
                    Content = content,
                    Opacity = 0.6,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                };
            }

            protected override Visual GetVisualChild(int index)
            {
                return _contentControl;
            }

            protected override int VisualChildrenCount
            {
                get { return 1; }
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                _contentControl.Arrange(new Rect(finalSize));
                return finalSize;
            }
        }
    }
}
