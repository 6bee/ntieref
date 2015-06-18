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
using ProductManager.Common.Domain.Model.ProductManager;

namespace ProductManager.WPF.Presentation.Views
{
    [Export(typeof(IProductListView))]
    public partial class ProductListView : UserControl, IProductListView
    {
        public ProductListView()
        {
            InitializeComponent();
            Loaded += ProductListView_Loaded;
            Unloaded += ProductListView_Unloaded;
        }

        void ProductListView_Unloaded(object sender, RoutedEventArgs e)
        {
            // move toolbar back from shell view to this view
            bool isRemoved = ((ShellWindow)ViewModel.ShellView).ToolBarTray.ToolBars.Remove(ToolBar);
            if (isRemoved)
            {
                ToolBarTray.ToolBars.Add(ToolBar);
            }
            isRemoved = ((ShellWindow)ViewModel.ShellView).ToolBarTray.ToolBars.Remove(FilterToolBar);
            if (isRemoved)
            {
                ToolBarTray.ToolBars.Add(FilterToolBar);
            }
        }

        void ProductListView_Loaded(object sender, RoutedEventArgs e)
        {
            // move toolbar from this view to shell view
            var isRemoved = ToolBarTray.ToolBars.Remove(ToolBar);
            if (isRemoved)
            {
                ((ShellWindow)ViewModel.ShellView).ToolBarTray.ToolBars.Add(ToolBar);
                ToolBar.DataContext = ViewModel;
            }
            isRemoved = ToolBarTray.ToolBars.Remove(FilterToolBar);
            if (isRemoved)
            {
                ((ShellWindow)ViewModel.ShellView).ToolBarTray.ToolBars.Add(FilterToolBar);
                FilterToolBar.DataContext = ViewModel;
            }
        }

        internal ProductListViewModel ViewModel { get { return DataContext as ProductListViewModel; } }

        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Product frame in e.RemovedItems)
            {
                ViewModel.SelectedProducts.Remove(frame);
            }
            foreach (Product frame in e.AddedItems)
            {
                ViewModel.SelectedProducts.Add(frame);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.InitData();
        }
    }
}
