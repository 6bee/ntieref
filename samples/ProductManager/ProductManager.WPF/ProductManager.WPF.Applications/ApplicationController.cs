using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using ProductManager.WPF.Applications.Controllers;
using System.ComponentModel;
using ProductManager.WPF.Applications.Services;
using ProductManager.WPF.Applications.Properties;
using ProductManager.WPF.Applications.ViewModels;

namespace ProductManager.WPF.Applications
{
    [Export]
    public class ApplicationController : Controller
    {
        private readonly IQuestionService questionService;
        private readonly IEntityController entityController;
        private readonly ProductController productController;
        private readonly ShellViewModel shellViewModel;
        private readonly DelegateCommand exitCommand;


        [ImportingConstructor]
        public ApplicationController(IQuestionService questionService, IPresentationController presentationController,
            IEntityController entityController, ProductController productController, ShellViewModel shellViewModel)
        {
            if (presentationController == null) { throw new ArgumentNullException("presentationController"); }
            if (shellViewModel == null) { throw new ArgumentNullException("shellViewModel"); }
            if (productController == null) { throw new ArgumentNullException("productController"); }

            presentationController.InitializeCultures();

            this.questionService = questionService;
            this.entityController = entityController;
            this.productController = productController;
            this.shellViewModel = shellViewModel;

            this.shellViewModel.Closing += ShellViewModelClosing;
            this.exitCommand = new DelegateCommand(Close);
        }


        public void Initialize()
        {
            shellViewModel.ExitCommand = exitCommand;

            entityController.Initialize();
            productController.Initialize();
        }

        public void Run()
        {
            shellViewModel.Show();
        }

        public void Shutdown()
        {
            entityController.Shutdown();
        }

        private void ShellViewModelClosing(object sender, CancelEventArgs e)
        {
            if (entityController.HasChanges)
            {
                if (shellViewModel.IsValid)
                {
                    bool? result = questionService.ShowQuestion(Resources.SaveChangesQuestion);
                    if (result == true)
                    {
                        if (!entityController.Save())
                        {
                            e.Cancel = true;
                        }
                    }
                    else if (result == null)
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = !questionService.ShowYesNoQuestion(Resources.LoseChangesQuestion);
                }
            }
        }

        private void Close()
        {
            shellViewModel.Close();
        }
    }
}
