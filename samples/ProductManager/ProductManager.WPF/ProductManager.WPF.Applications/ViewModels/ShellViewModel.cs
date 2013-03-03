using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using ProductManager.WPF.Applications.Views;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Windows.Input;
using System.ComponentModel;
using System.Globalization;
using ProductManager.WPF.Applications.Properties;

namespace ProductManager.WPF.Applications.ViewModels
{
    [Export]
    public class ShellViewModel : ViewModel<IShellView>
    {
        private readonly IMessageService messageService;
        private readonly DelegateCommand aboutCommand;
        private ICommand saveCommand;
        private ICommand exitCommand;
        private bool canSave;
        private bool isValid = true;
        private object productListView;
        private object productView;


        [ImportingConstructor]
        public ShellViewModel(IShellView view, IMessageService messageService)
            : base(view)
        {
            this.messageService = messageService;
            this.aboutCommand = new DelegateCommand(ShowAboutMessage);

            view.Closing += ViewClosing;
        }


        public static string Title { get { return ApplicationInfo.ProductName; } }

        private Cursor uiCursor = Cursors.Arrow;
        public Cursor UICursor
        {
            get
            {
                return uiCursor;
            }
            private set
            {
                if (value != uiCursor)
                {
                    uiCursor = value;
                    RaisePropertyChanged("UICursor");
                }
            }
        }

        internal void SetWaitCursor()
        {
            UICursor = Cursors.Wait;
        }

        internal void ReleaseWaitCursor()
        {
            UICursor = Cursors.Arrow;
        }


        public ICommand AboutCommand { get { return aboutCommand; } }
        
        public ICommand SaveCommand
        {
            get { return saveCommand; }
            set
            {
                if (saveCommand != value)
                {
                    saveCommand = value;
                    RaisePropertyChanged("SaveCommand");
                }
            }
        }

        public ICommand ExitCommand
        {
            get { return exitCommand; }
            set
            {
                if (exitCommand != value)
                {
                    exitCommand = value;
                    RaisePropertyChanged("ExitCommand");
                }
            }
        }

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

        public object ProductListView
        {
            get { return productListView; }
            set
            {
                if (productListView != value)
                {
                    productListView = value;
                    RaisePropertyChanged("ProductListView");
                }
            }
        }

        public object ProductView
        {
            get { return productView; }
            set
            {
                if (productView != value)
                {
                    productView = value;
                    RaisePropertyChanged("ProductView");
                }
            }
        }

        private bool _productHasChanges = false;
        public bool ProductHasChanges
        {
            get
            {
                //return Products != null && Products.Any(p => p.HasChanges);
                return _productHasChanges;
            }
            internal set
            {
                if (_productHasChanges != value)
                {
                    _productHasChanges = value;
                    RaisePropertyChanged("ProductHasChanges");
                }
            }
        }


        public event CancelEventHandler Closing;


        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (Closing != null) { Closing(this, e); }
        }

        private void ViewClosing(object sender, CancelEventArgs e)
        {
            OnClosing(e);
        }

        private void ShowAboutMessage()
        {
            messageService.ShowMessage(string.Format(CultureInfo.CurrentCulture, Resources.AboutText,
                ApplicationInfo.ProductName, ApplicationInfo.Version));
        }
    }
}
