using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using ProductManager.WPF.Applications.Views;
using ProductManager.WPF.Applications.ViewModels;

namespace ProductManager.WPF.Presentation.Views
{
    [Export(typeof(IShellView))]
    public partial class ShellWindow : Window, IShellView
    {
        private readonly HashSet<ValidationError> errors = new HashSet<ValidationError>();


        public ShellWindow()
        {
            InitializeComponent();

            Validation.AddErrorHandler(this, ErrorChangedHandler);
        }


        private ShellViewModel ViewModel { get { return DataContext as ShellViewModel; } }


        private void ErrorChangedHandler(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                errors.Add(e.Error);
            }
            else
            {
                errors.Remove(e.Error);
            }

            ViewModel.IsValid = !errors.Any();
        }
    }
}
