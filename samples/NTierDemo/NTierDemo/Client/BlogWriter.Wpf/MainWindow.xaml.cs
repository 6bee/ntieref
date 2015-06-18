using BlogWriter.Wpf.ViewModels;
using NTierDemo.Client.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlogWriter.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Func<INTierDemoDataContext> dataContextFactory = () => 
                new NTierDemoDataContext()
                {
                    // Note: we're required to unset the dispatcher to avoid dead-locking when scheduling from background to main thread as we use async/await
                    //       once a proper .NET 4.5 implementation of the n-tier entity framework we don't need to do this any longer
                    Dispatcher = null
                };

            DataContext = new MainViewModel(dataContextFactory);
        }
    }
}
