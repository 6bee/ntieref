using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;

namespace ProjectTemplateWizard
{
    public class NTierEntityFrameworkWizard : IWizard
    {
        private static readonly string[] ProjectFolder = { "Client", "Common", "Server", "Silverlight" };
        //private static readonly string[] Resources = 
        //{
        //    "Remote.Linq.dll", 
        //    "Remote.Linq.pdb", 
        //    "NTier.Client.Domain.dll", 
        //    "NTier.Client.Domain.pdb", 
        //    "NTier.Common.Domain.dll", 
        //    "NTier.Common.Domain.pdb",
        //    "NTier.Server.Domain.dll", 
        //    "NTier.Server.Domain.pdb", 
        //};
        private static readonly string[] ResourceArchives = 
        {
            "Lib_NET.zip", 
        };

        private static string solutionname = "";
        private static string safesolutionname = "";

        private DTE2 dte = null;

        // This method is called before opening any item that has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        // This method is only called for item templates, not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        // This method is called after the project is created.
        public void RunFinished()
        {
            // if solution
            if (dte != null)
            {
                // set startup project
                try
                {
                    // get startup project
                    var project = dte.GetProjects().First(p => p.UniqueName.Contains(safesolutionname + ".Client.Domain.Test"));

                    // set startup project
                    SolutionBuild2 sb = (SolutionBuild2)dte.Solution.SolutionBuild;
                    sb.StartupProjects = new object[] { project.UniqueName };
                }
                catch { }

                // collapse projects
                try
                {
                    dte.CollapseAll(exlusionList: ProjectFolder);
                }
                catch { }

                // copy lib
                try
                {
                    var directory = Path.GetDirectoryName(dte.GetProjects().First().FullName);
                    directory = Path.Combine(directory, @"..\..");
                    //directory.CreateLib(Resources, true);
                    directory.UnpackZipToLib(ResourceArchives);
                }
                catch { }
            }
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            // run for multi project
            if (runKind == WizardRunKind.AsMultiProject)
            {
                dte = (DTE2)automationObject;

                // read project name
                solutionname = replacementsDictionary["$projectname$"];
                safesolutionname = replacementsDictionary["$safeprojectname$"].Replace(" ", "_");
            }

            // save project name as solution name
            replacementsDictionary.Add("$solutionname$", solutionname);
            replacementsDictionary.Add("$safesolutionname$", safesolutionname);
        }

        // This method is only called for item templates, not for project templates.
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
