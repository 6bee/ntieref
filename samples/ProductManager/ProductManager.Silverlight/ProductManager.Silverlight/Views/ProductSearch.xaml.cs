using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProductManager.Client.Domain;
using ProductManager.Common.Domain.Model.ProductManager;
using NTier.Client.Domain;

namespace ProductManager.Silverlight.Views
{
    public partial class ProductSearch : UserControl
    {
        private readonly ProductManagerDataContext _productManagerDataContext;

        public ProductSearch()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // use of data loader and entity collection view to bind data to grid
                // enabling automatic remote filtering, sorting and paging
                _productManagerDataContext = new ProductManagerDataContext() { MergeOption = MergeOption.PreserveChanges, DetachEntitiesUponNewQueryResult = true };
                this.DataContext = _productManagerDataContext;

                //_productManagerDataContext.Products.MergeOption = MergeOption.PreserveChanges;
                var queryable = _productManagerDataContext.Products.AsQueryable();
                var loader = new RemoteDataLoader<Product>(queryable) { RequestDelay = TimeSpan.FromSeconds(0.02) };
                var collectionView = new EditableEntityCollectionView<Product>(loader)
                {
                    CanFilter = false, // prevent auto filter feature of data grid --> CanFilter = false
                    InitializeNewItem = delegate(Product p)
                    {
                        p.SellStartDate = DateTime.Now.Date;
                    },
                    CanChangePageWhenHasChanges = true // default is false
                }; 
                collectionView.SortDescriptions.Add(new SortDescription("ProductID", ListSortDirection.Ascending));

                searchDialog.DataContext = collectionView;

                _productManagerDataContext.SaveChangesCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        throw new Exception("Saving failed", e.Error);
                    }
                };

                //// alternatively queries may be built manually using linq
                //// create data context
                //var ctx = new ProductManagerDataContext() { MergeOption = MergeOption.PreserveChanges };
                //// create query
                //var query = ctx.Products.AsQueryable()
                //    .IncludeTotalCount()
                //    .Include("ProductCategory")
                //    .Skip(30)
                //    .Take(10)
                //    .OrderBy(p => p.ProductCategory.Name)
                //    .ThenBy(p => p.Name)
                //    .Where(p => p.ListPrice >= 100m);
                //// execute query asynchronously
                //query.ExecuteAsync(
                //    (result) =>
                //    {
                //        // check for exception
                //        if (result.IsFaulted) throw new Exception("Query failed", Exresult.Exception);

                //        // access result data
                //        var totalCount = result.EntitySet.TotalCount;
                //        var dataSource = result.EntitySet;
                //        DataContext = dataSource;
                //    });
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _productManagerDataContext.SaveChangesAsync();
        }

        private void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            // get products from local data context, having changes and revert to original values
            foreach (var product in _productManagerDataContext.Products.Where(p => p.HasChanges))
            {
                product.RevertChanges();
            }
        }
    }
}
