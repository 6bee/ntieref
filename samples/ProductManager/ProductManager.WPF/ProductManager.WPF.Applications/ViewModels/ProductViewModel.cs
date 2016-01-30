using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using ProductManager.WPF.Applications.Views;
using ProductManager.Common.Domain.Model.ProductManager;
using System.Windows.Input;
using System.Collections.ObjectModel;
using NTier.Common.Domain.Model;

namespace ProductManager.WPF.Applications.ViewModels
{
    [Export]
    public class ProductViewModel : ViewModel<IProductView>
    {
        private bool isValid = true;
        private Product product;
        private readonly DelegateCommand revertChangesCommand;

        [ImportingConstructor]
        public ProductViewModel(IProductView view)
            : base(view)
        {
            revertChangesCommand = new DelegateCommand(delegate
                {
                    if (Product != null && Product.ChangeTracker.State == ObjectState.Modified && Product.HasChanges)
                    {
                        Product.RevertChanges();
                    }
                },
                () => Product != null && Product.ChangeTracker.State == ObjectState.Modified && Product.HasChanges);

            PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "Product":
                        RaisePropertyChanged("IsEnabled");
                        revertChangesCommand.RaiseCanExecuteChanged();
                        break;
                }
            };
        }

        public bool IsEnabled { get { return Product != null; } }

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }

        public Product Product
        {
            get { return product; }
            set
            {
                if (product != value)
                {
                    // de-register property changed handler
                    if (product != null)
                    {
                        product.PropertyChanged -= product_PropertyChanged;
                    }

                    product = value;

                    // register property changed handler
                    if (product != null)
                    {
                        product.PropertyChanged += product_PropertyChanged;
                    }

                    RaisePropertyChanged("Product");
                }
            }
        }

        void product_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "HasChanges":
                    revertChangesCommand.RaiseCanExecuteChanged();
                    break;
            }
        }

        private IEnumerable<ProductCategory> _categories;
        public IEnumerable<ProductCategory> Categories
        {
            get { return _categories; }
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    RaisePropertyChanged("Categories");
                }
            }
        }

        public ICommand RevertChangesCommand { get { return revertChangesCommand; } }

        public void Focus()
        {
            ViewCore.FocusFirstControl();
        }
    }
}