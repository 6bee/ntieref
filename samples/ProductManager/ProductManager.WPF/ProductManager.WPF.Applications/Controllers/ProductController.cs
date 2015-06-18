using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Waf.Applications;
using ProductManager.Client.Domain;
using ProductManager.Common.Domain.Model.ProductManager;
using ProductManager.WPF.Applications.Services;
using ProductManager.WPF.Applications.ViewModels;
using ProductManager.WPF.Applications.Views;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;

namespace ProductManager.WPF.Applications.Controllers
{
    [Export]
    public class ProductController : Controller
    {
        private enum SortingDirection
        {
            Ascendung,
            Descending
        }

        private readonly CompositionContainer container;
        private readonly IEntityService entityService;
        private readonly ShellViewModel shellViewModel;
        private readonly ProductViewModel productViewModel;
        private readonly DelegateCommand firstPageCommand;
        private readonly DelegateCommand previousPageCommand;
        private readonly DelegateCommand nextPageCommand;
        private readonly DelegateCommand lastPageCommand;
        private readonly DelegateCommand sortCommand;
        private readonly DelegateCommand filterColorCommand;
        private readonly DelegateCommand addNewCommand;
        private readonly DelegateCommand removeCommand;
        private ProductListViewModel productListViewModel;

        private string sortingColumn = null;
        private SortingDirection sortDirection = SortingDirection.Ascendung;
        private ProductColorFilter colorFilter = ProductColorFilter.NoFilter;

        private readonly System.Windows.Threading.Dispatcher Dispatcher;

        [ImportingConstructor]
        public ProductController(CompositionContainer container, IEntityService entityService, ShellViewModel shellViewModel, ProductViewModel productViewModel)
        {
            this.container = container;
            this.entityService = entityService;
            this.shellViewModel = shellViewModel;
            this.productViewModel = productViewModel;
            this.firstPageCommand = new DelegateCommand(FirstPage, HasFirstPage);
            this.previousPageCommand = new DelegateCommand(PreviousPage, HasPreviousPage);
            this.nextPageCommand = new DelegateCommand(NextPage, HasNextPage);
            this.lastPageCommand = new DelegateCommand(LastPage, HasLastPage);
            this.sortCommand = new DelegateCommand(Sort, CanSort);
            this.filterColorCommand = new DelegateCommand(FilterColor, CanFilterColor);
            this.addNewCommand = new DelegateCommand(AddNewProduct, CanAddNewProduct);
            this.removeCommand = new DelegateCommand(RemoveProduct, CanRemoveProduct);

            this.Dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        }

        public void Initialize()
        {
            IProductListView productListView = container.GetExportedValue<IProductListView>();
            productListViewModel = new ProductListViewModel(productListView);
            productListViewModel.ShellView = shellViewModel.View;
            productListViewModel.FirstPageCommand = firstPageCommand;
            productListViewModel.PreviousPageCommand = previousPageCommand;
            productListViewModel.NextPageCommand = nextPageCommand;
            productListViewModel.LastPageCommand = lastPageCommand;
            productListViewModel.SortCommand = sortCommand;
            productListViewModel.FilterColorCommand = filterColorCommand;
            productListViewModel.AddNewCommand = addNewCommand;
            productListViewModel.RemoveCommand = removeCommand;
            productListViewModel.PropertyChanged += (sender, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case "SelectedProduct":
                            // start re-query product to get latest changes
                            // this call may potentially include further relations as required by the detail view
                            if (productListViewModel.SelectedProduct != null && 
                                productListViewModel.SelectedProduct.ChangeTracker.State != ObjectState.Added)
                            {
                                entityService.Products
                                             .AsQueryable()
                                             .Include("ProductCategory")
                                             .Where(p => p.ProductID == productListViewModel.SelectedProduct.ProductID)
                                             .ExecuteAsync(callback: delegate(ICallbackResult<Product> result) { /* note: pass callback or use task to handle potential exceptions */ });
                            }
                            // set selected product for detail view
                            productViewModel.Product = productListViewModel.SelectedProduct;
                            UpdateCommands();
                            break;
                        case "PageSize":
                        case "PageNumber":
                        //case "NumberOfPages":
                        //case "ColorFilter":
                            LoadProducts();
                            UpdatePageCommands();
                            break;
                        case "NumberOfPages":
                            UpdatePageCommands();
                            break;
                        case "Products":
                            // release waiting cursor
                            shellViewModel.ReleaseWaitCursor();
                            break;
                    }
                };

            entityService.Products.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanges")
                {
                    var hasChanges = entityService.Products.HasChanges;
                    shellViewModel.ProductHasChanges = hasChanges;
                }
            };

            shellViewModel.ProductListView = productListViewModel.View;
            shellViewModel.ProductView = productViewModel.View;

            productListViewModel.InitialDataLoad += (s, e) =>
            {
                LoadCategories();
                LoadProducts();
            };
        }

        private void LoadCategories()
        {
            // asynchronous data request
            entityService.Categories
                         .AsQueryable()
                         .OrderBy(c => c.Name) // default order
                         .ExecuteAsync(SetProductCategories);
        }

        private void LoadProducts()
        {
            // set waiting cursor
            shellViewModel.SetWaitCursor();

            // get data service queriable
            var products = entityService.Products
                                        .AsQueryable()
                                        .OrderBy(p => p.ProductID) // default order
                                        .Take(10) // default page size
                                        .Include("ProductCategory");

            // set sorting
            if (!string.IsNullOrWhiteSpace(sortingColumn))
            {
                products = sortDirection == SortingDirection.Descending ? products.OrderByDescending(p => p[sortingColumn]) : products.OrderBy(p => p[sortingColumn]);
            }

            // set paging
            products = products
                    .Skip((productListViewModel.PageNumber - 1) * productListViewModel.PageSize)
                    .Take(productListViewModel.PageSize);

            // set filter
            if (colorFilter != ProductColorFilter.NoFilter)
            {
                var color = colorFilter.ToString();
                products = products.Where(p => p.Color == color);
            }

            // include total count
            products = products.IncludeTotalCount();


            // synchronous data request
            //productListViewModel.Products = products.ToObservableCollection();
            //if (products.TotalCount.HasValue)
            //{
            //    productListViewModel.TotalNumberOfProducts = products.TotalCount.Value;
            //}

            // asynchronous data request
            products.ExecuteAsync(SetProducts);
        }

        private void SetProductCategories(ICallbackResult<ProductCategory> result)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action<ICallbackResult<ProductCategory>>)SetProductCategories, result);
                return;
            }

            if (result.IsFaulted)
            {
                throw new Exception("Failed loading product categories", result.Exception);
            }

            productViewModel.Categories = result.EntitySet.ToObservableCollection();
        }

        private void SetProducts(ICallbackResult<Product> result)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action<ICallbackResult<Product>>)SetProducts, result);
                return;
            }

            if (result.IsFaulted)
            {
                throw new Exception("Failed loading products", result.Exception);
            }

            productListViewModel.Products = result.EntitySet.ToObservableCollection();

            if (result.EntitySet.TotalCount.HasValue)
            {
                productListViewModel.TotalNumberOfProducts = result.EntitySet.TotalCount.Value;
            }
        }


        private bool HasFirstPage() { return HasPreviousPage(); }
        private bool HasPreviousPage() { return productListViewModel.PageNumber > 1; }
        private bool HasNextPage() { return productListViewModel.PageNumber < productListViewModel.NumberOfPages && productListViewModel.NumberOfPages > 1; }
        private bool HasLastPage() { return HasNextPage(); }

        private void FirstPage()
        {
            productListViewModel.PageNumber = 1;
        }
        private void PreviousPage()
        {
            productListViewModel.PageNumber--;
        }
        private void NextPage()
        {
            productListViewModel.PageNumber++;
        }
        private void LastPage()
        {
            productListViewModel.PageNumber = productListViewModel.NumberOfPages;
        }

        private bool CanSort(object columnName) { return columnName is string && new string[] { "ProductID", "Name", "Color" }.Contains(columnName); }
        private void Sort(object columnName)
        {
            if (CanSort(columnName))
            {
                sortDirection = sortingColumn == (string)columnName && sortDirection == SortingDirection.Ascendung ?
                    sortDirection =  SortingDirection.Descending : sortDirection = SortingDirection.Ascendung;
                sortingColumn = (string)columnName;

                ResetPageing();
            }
        }

        private bool CanFilterColor(object filter) { return filter is ProductColorFilter; }
        private void FilterColor(object filter)
        {
            if (CanFilterColor(filter))
            {
                colorFilter = (ProductColorFilter)filter;

                ResetPageing();
            }
        }

        private void ResetPageing()
        {
            productListViewModel.PageNumber = int.MaxValue;
            FirstPage();
        }

        private bool CanAddNewProduct() { return true; }
        private void AddNewProduct()
        {
            Product product = new Product()
            {
                SellStartDate = DateTime.Now.Date
                //rowguid = Guid.NewGuid()
            };
            productListViewModel.Products.Add(product);

            productListViewModel.SelectedProduct = product;
            productViewModel.Focus();
        }

        private bool CanRemoveProduct() { return productListViewModel.SelectedProduct != null; }
        private void RemoveProduct()
        {
            foreach (Product product in productListViewModel.SelectedProducts.ToArray())
            {
                productListViewModel.Products.Remove(product);
            }
        }

        private void UpdateCommands()
        {
            addNewCommand.RaiseCanExecuteChanged();
            removeCommand.RaiseCanExecuteChanged();
        }
        
        private void UpdatePageCommands()
        {
            firstPageCommand.RaiseCanExecuteChanged();
            previousPageCommand .RaiseCanExecuteChanged();
            nextPageCommand.RaiseCanExecuteChanged();
            lastPageCommand.RaiseCanExecuteChanged();
        }
    }
}
