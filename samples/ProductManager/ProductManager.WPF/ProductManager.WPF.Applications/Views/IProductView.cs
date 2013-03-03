using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;

namespace ProductManager.WPF.Applications.Views
{
    public interface IProductView : IView
    {
        void FocusFirstControl();
    }
}
