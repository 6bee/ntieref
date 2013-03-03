using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Windows.Input;
using ProductManager.Client.Domain;
using ProductManager.Common.Domain.Model.ProductManager;
using ProductManager.WPF.Applications.Views;

namespace ProductManager.WPF.Applications.ViewModels
{
    public class ProductListViewModel : ViewModel<IProductListView>
    {
        private ObservableCollection<Product> products;
        private readonly ObservableCollection<Product> selectedProducts;
        private Product selectedProduct;
        private ICommand firstPageCommand;
        private ICommand previousPageCommand;
        private ICommand nextPageCommand;
        private ICommand lastPageCommand;
        private ICommand sortCommand;
        private ICommand filterCommand;
        private ICommand addNewCommand;
        private ICommand removeCommand;

        public event EventHandler InitialDataLoad;

        public ProductListViewModel(IProductListView view)
            : base(view)
        {
            this.selectedProducts = new ObservableCollection<Product>();
        }

        public void InitData()
        {
            if (InitialDataLoad != null)
            {
                InitialDataLoad(this, EventArgs.Empty);
            }
        }

        private object shellView;
        public object ShellView
        {
            get { return shellView; }
            set
            {
                if (shellView != value)
                {
                    shellView = value;
                    RaisePropertyChanged("ShellView");
                }
            }
        }

        public int PageSize
        {
            get { return pageSize; }
            internal set
            {
                if (pageSize != value && value > 0)
                {
                    pageSize = value;
                    RaisePropertyChanged("PageSize");
                }
            }
        }
        private int pageSize = 15;

        public int PageNumber
        {
            get { return pageNumber; }
            set
            {
                if (pageNumber != value && value > 0)
                {
                    pageNumber = value;
                    if (pageNumber != int.MaxValue)
                    {
                        RaisePropertyChanged("PageNumber");
                    }
                }
            }
        }
        private int pageNumber = 1;

        private void ResetPageing()
        {
            PageNumber = int.MaxValue;
            PageNumber = 1;
        }

        public int NumberOfPages
        {
            get
            {
                return (int)((TotalNumberOfProducts / PageSize) + (TotalNumberOfProducts % PageSize > 0 ? 1 : 0));
            }
        }


        private long totalNumberOfProducts = 0;
        public long TotalNumberOfProducts
        {
            get
            {
                return totalNumberOfProducts;
            }
            set
            {
                if (totalNumberOfProducts != value)
                {
                    totalNumberOfProducts = value;
                    RaisePropertyChanged("TotalNumberOfProducts");
                    RaisePropertyChanged("NumberOfPages");
                }
            }
        }

        public ObservableCollection<Product> Products
        {
            get { return products; }
            internal set
            {
                products = value;
                RaisePropertyChanged("Products");

                SelectedProduct = products.FirstOrDefault();
            }
        }
        
        public ObservableCollection<Product> SelectedProducts
        {
            get { return selectedProducts; }
        }

        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                if (selectedProduct != value)
                {
                    selectedProduct = value;
                    RaisePropertyChanged("SelectedProduct");
                }
            }
        }

        public ICommand SortCommand
        {
            get { return sortCommand; }
            set
            {
                if (sortCommand != value)
                {
                    sortCommand = value;
                    RaisePropertyChanged("SortCommand");
                }
            }
        }

        public ICommand FilterColorCommand
        {
            get { return filterCommand; }
            set
            {
                if (filterCommand != value)
                {
                    filterCommand = value;
                    RaisePropertyChanged("FilterColorCommand");
                }
            }
        }

        public ICommand FirstPageCommand
        {
            get { return firstPageCommand; }
            set
            {
                if (firstPageCommand != value)
                {
                    firstPageCommand = value;
                    RaisePropertyChanged("FirstPageCommand");
                }
            }
        }

        public ICommand PreviousPageCommand
        {
            get { return previousPageCommand; }
            set
            {
                if (previousPageCommand != value)
                {
                    previousPageCommand = value;
                    RaisePropertyChanged("PreviousPageCommand");
                }
            }
        }

        public ICommand NextPageCommand
        {
            get { return nextPageCommand; }
            set
            {
                if (nextPageCommand != value)
                {
                    nextPageCommand = value;
                    RaisePropertyChanged("NextPageCommand");
                }
            }
        }

        public ICommand LastPageCommand
        {
            get { return lastPageCommand; }
            set
            {
                if (lastPageCommand != value)
                {
                    lastPageCommand = value;
                    RaisePropertyChanged("LastPageCommand");
                }
            }
        }

        public ICommand AddNewCommand
        {
            get { return addNewCommand; }
            set
            {
                if (addNewCommand != value)
                {
                    addNewCommand = value;
                    RaisePropertyChanged("AddNewCommand");
                }
            }
        }

        public ICommand RemoveCommand
        {
            get { return removeCommand; }
            set
            {
                if (removeCommand != value)
                {
                    removeCommand = value;
                    RaisePropertyChanged("RemoveCommand");
                }
            }
        }
    }
}
