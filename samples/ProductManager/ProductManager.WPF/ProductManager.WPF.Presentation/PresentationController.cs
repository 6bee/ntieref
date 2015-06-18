using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using ProductManager.WPF.Applications.Controllers;
using System.Windows;
using System.Globalization;
using System.Windows.Markup;

namespace ProductManager.WPF.Presentation
{
    [Export(typeof(IPresentationController))]
    public class PresentationController : IPresentationController
    {
        public void InitializeCultures()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
    }
}
