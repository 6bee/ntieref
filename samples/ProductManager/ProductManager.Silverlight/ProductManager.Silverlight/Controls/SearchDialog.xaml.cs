using C1.Silverlight.DataGrid;
using NTier.Client.Domain;
using ProductManager.Common.Domain.Model.ProductManager;
using System;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ProductManager.Silverlight.Controls
{
    public partial class SearchDialog : UserControl
    {
        private readonly object FilterBoxLock = new object();
        private readonly DispatcherTimer StatusTimer;
        private int statusDotsCount = 0;
        private Expression<Func<Product, bool>> filterExpression;

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
                        var filters = collectionView.FilterExpressions;

                        if (filterExpression != null)
                        {
                            filters.Remove(filterExpression);
                        }

                        if (string.IsNullOrEmpty(filterBox.Text))
                        {
                            filterExpression = null;
                        }
                        else
                        {
                            var filterValue = filterBox.Text;
                            filterExpression = _ => _.ProductID.ToString().Contains(filterValue)
                                || _.Name.Contains(filterValue)
                                || _.ProductNumber.Contains(filterValue)
                                || _.Color.Contains(filterValue)
                                || _.StandardCost.ToString().Contains(filterValue)
                                || _.ListPrice.ToString().Contains(filterValue)
                                || _.Size.ToString().Contains(filterValue)
                                || _.Weight.ToString().Contains(filterValue)
                                || ((_.SellStartDate.Day < 10 ? "0" : "") + _.SellStartDate.Day +
                                    (_.SellStartDate.Month < 10 ? ".0" : ".") + _.SellStartDate.Month +
                                    "." + _.SellStartDate.Year).Contains(filterValue)
                                || (_.SellEndDate.HasValue &&
                                    ((_.SellEndDate.Value.Day < 10 ? "0" : "") + _.SellEndDate.Value.Day +
                                    (_.SellEndDate.Value.Month < 10 ? ".0" : ".") + _.SellEndDate.Value.Month +
                                    "." + _.SellEndDate.Value.Year).Contains(filterValue))
                                //|| (_.DiscontinuedDate.HasValue &&
                                //    ((_.DiscontinuedDate.Value.Day < 10 ? "0" : "") + _.DiscontinuedDate.Value.Day +
                                //    (_.DiscontinuedDate.Value.Month < 10 ? ".0" : ".") + _.DiscontinuedDate.Value.Month +
                                //    "." + _.DiscontinuedDate.Value.Year).Contains(filterValue))
                                ;
                            filters.Add(filterExpression);
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

        private void C1DataGrid_AutoGeneratingColumn(object sender, C1.Silverlight.DataGrid.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridDateTimeColumn)
            {
                e.Column.Format = "dd.MM.yyyy";
            }
        }
    }
}
