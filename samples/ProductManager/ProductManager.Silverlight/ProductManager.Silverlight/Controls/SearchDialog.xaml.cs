using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ProductManager.Client.Domain;
using NTier.Client.Domain;

namespace ProductManager.Silverlight.Controls
{
    public partial class SearchDialog : UserControl
    {
        private readonly object FilterBoxLock = new object();
        private readonly DispatcherTimer StatusTimer;
        private int statusDotsCount = 0;

        public SearchDialog()
        {
            InitializeComponent();

            // filter box
            this.filterBox.TextChanged += (s, e) =>
            {
                lock (FilterBoxLock)
                {
                    var collectionView = DataContext as IFilteredCollectionView;
                    if (collectionView != null)
                    {
                        var filters = collectionView.FilterDescriptions;
                        var filter = filters.FirstOrDefault(f => f.PropertyName == null);
                        if (filter != null)
                        {
                            filters.Remove(filter);
                        }
                        if (!string.IsNullOrEmpty(filterBox.Text))
                        {
                            filters.Add(new FilterDescription { PropertyName = null, Value = filterBox.Text, FilterOperation = FilterOperation.Contains });
                        }
                    }
                }
            };

            // status label
            this.StatusTimer = new DispatcherTimer();
            this.StatusTimer.Interval = TimeSpan.FromSeconds(0.2);
            this.StatusTimer.Tick += (sender, e) =>
            {
                lock (StatusTimer)
                {
                    if (!StatusTimer.IsEnabled) return;

                    statusDotsCount = ++statusDotsCount % 20;
                    var dots = "";
                    for (int i = 0; i < statusDotsCount; i++)
                    {
                        dots += " .";
                    }
                    this.statusLabel.Content = "Loading" + dots;
                }
            };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var collectionView = DataContext as IEntityCollectionView;
            if (collectionView != null)
            {
                collectionView.DataLoading += (s, e1) =>
                {
                    lock (StatusTimer)
                    {
                        statusDotsCount = 0;
                        StatusTimer.Start();
                    }
                };
                collectionView.DataLoaded += (s, e1) =>
                {
                    lock (StatusTimer)
                    {
                        StatusTimer.Stop();
                        statusLabel.Content = string.Format("Total Count: {0}", collectionView.TotalItemCount);
                    }
                };
            }
        }
    }
}
