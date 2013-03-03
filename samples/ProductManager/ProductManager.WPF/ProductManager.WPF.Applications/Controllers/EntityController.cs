using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Xml.Linq;
using ProductManager.Client.Domain;
using ProductManager.WPF.Applications.Properties;
using ProductManager.WPF.Applications.Services;
using ProductManager.WPF.Applications.ViewModels;
using NTier.Client.Domain;

namespace ProductManager.WPF.Applications.Controllers
{
    /// <summary>
    /// This controller is responsible for the database connection and the save operation.
    /// </summary>
    [Export(typeof(IEntityController))]
    internal class EntityController : Controller, IEntityController
    {
        private readonly EntityService entityService;
        private readonly IMessageService messageService;
        private readonly ShellViewModel shellViewModel;
        private readonly DelegateCommand saveCommand;
        private ProductManagerDataContext context;


        [ImportingConstructor]
        public EntityController(EntityService entityService, IMessageService messageService, ShellViewModel mainViewModel)
        {
            this.entityService = entityService;
            this.messageService = messageService;
            this.shellViewModel = mainViewModel;
            this.saveCommand = new DelegateCommand(() => Save(), CanSave);
        }


        public bool HasChanges
        {
            get { return context != null && context.HasChanges; }
        }


        public void Initialize()
        {
            if (System.Windows.Threading.Dispatcher.CurrentDispatcher.CheckAccess())
            {

            }

            context = new ProductManagerDataContext() { MergeOption = MergeOption.PreserveChanges };
            entityService.Context = context;

            shellViewModel.PropertyChanged += ShellViewModelPropertyChanged;
            shellViewModel.SaveCommand = saveCommand;
        }

        public void Shutdown()
        {
            //context.Dispose();
        }

        public bool CanSave() { return shellViewModel.IsValid && shellViewModel.ProductHasChanges; }

        public bool Save()
        {
            bool saved = false;
            if (!CanSave())
            {
                throw new InvalidOperationException("You must not call Save when CanSave returns false.");
            }
            try
            {
                shellViewModel.SetWaitCursor();
                context.SaveChanges();
                shellViewModel.ReleaseWaitCursor();
                saved = true;
            }
            catch (ValidationException e)
            {
                messageService.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.SaveErrorInvalidEntities, e.Message));
            }
            catch (UpdateException e)
            {
                messageService.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.SaveErrorInvalidFields, e.InnerException.Message));
            }
            return saved;
        }

        private void ShellViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsValid":
                case "ProductHasChanges":
                    saveCommand.RaiseCanExecuteChanged();
                    break;
            }
        }
    }
}
