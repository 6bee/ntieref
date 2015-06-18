using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;

namespace ProjectTemplateWizard
{
    public class NTierEntityFrameworkSilverlightLibWizard : IWizard
    {
        private static readonly IWizard NuGetWizard = (IWizard)Assembly
            .Load("NuGet.VisualStudio.Interop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
            .CreateInstance("NuGet.VisualStudio.TemplateWizard");

        private static readonly Regex UnsafeCharRegex = new Regex(@"[^a-zA-Z0-9_\-\.]");
        //private static readonly string[] Resources = 
        //{
        //    "Microsoft.CSharp.dll",
        //    "Microsoft.CSharp.xml",
        //    "Remote.Linq.SL.dll", 
        //    "Remote.Linq.SL.pdb", 
        //    "NTier.Silverlight.Domain.dll", 
        //    "NTier.Silverlight.Domain.pdb", 
        //};
        //private static readonly string[] ResourceArchives = 
        //{
        //    "Lib_SL.zip", 
        //};

        private DTE2 dte = null;


        // This method is called before opening any item that has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
            NuGetWizard.BeforeOpeningFile(projectItem);
        }

        public void ProjectFinishedGenerating(Project project)
        {
            NuGetWizard.ProjectFinishedGenerating(project);

            // copy lib
            try
            {
                var solutiondirectory = Path.GetDirectoryName(dte.Solution.FullName);
                var projectDirectory = Path.GetDirectoryName(project.FullName);
                var directory = Path.Combine(projectDirectory, @"..\..");

                if (!Directory.Exists(directory) || directory.IsParentDirectoryOf(solutiondirectory))
                {
                    directory = solutiondirectory;
                }

                //directory.CreateLib(Resources, true);
                //directory.UnpackZipToLib(ResourceArchives);
            }
            catch { }
        }

        // This method is only called for item templates, not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            NuGetWizard.ProjectItemFinishedGenerating(projectItem);
        }

        // This method is called after the project is created.
        public void RunFinished()
        {
            NuGetWizard.RunFinished();
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            NuGetWizard.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            dte = (DTE2)automationObject;
            var solutionName = Path.GetFileNameWithoutExtension(dte.Solution.FullName);
            var safesolutionName = UnsafeCharRegex.Replace(solutionName.Replace(" ", "_"), string.Empty);

            var form = new SilverlightLibWizard(safesolutionName);
            var dialogResult = form.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                throw new Microsoft.VisualStudio.TemplateWizard.WizardCancelledException();
            }

            var solutionBaseNamespace = form.SolutionBaseNamespace;

            replacementsDictionary.Add("$solutionbasenamespace$", solutionBaseNamespace);
        }

        // This method is only called for item templates, not for project templates.
        public bool ShouldAddProjectItem(string filePath)
        {
            //NuGetWizard.ShouldAddProjectItem(filePath);
            return true;
        }
    }
}
