using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace ProjectTemplateWizard
{
    internal static class SolutionExtensions
    {
        public static IList<Project> GetProjects(this DTE2 dte)
        {
            List<Project> projects = new List<Project>();
            foreach (Project project in dte.Solution.Projects)
            {
                if (project.Kind.Equals(EnvDTE80ProjectKinds.vsProjectKindSolutionFolder, StringComparison.OrdinalIgnoreCase))
                {
                    projects.AddRange(GetProjects(project.ProjectItems));
                }
                else
                {
                    projects.Add(project);
                }
            }
            return projects;
        }

        public static IList<Project> GetProjects(this ProjectItems projectItems)
        {
            List<Project> projects = new List<Project>();
            foreach (ProjectItem item in projectItems)
            {
                Project project = item.SubProject;
                if (project == null)
                {
                    continue;
                }
                else if (project.IsSolutionFolder())
                {
                    projects.AddRange(GetProjects(project.ProjectItems));
                }
                else
                {
                    projects.Add(project);
                }
            }
            return projects;
        }

        public static bool IsSolutionFolder(this Project project)
        {
            // project.Object is SolutionFolder
            return project.Kind.Equals(EnvDTE80ProjectKinds.vsProjectKindSolutionFolder, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSolutionFolder(this ProjectItem projectItem)
        {
            return projectItem.Kind.Equals(EnvDTE80ProjectKinds.vsProjectKindSolutionFolder, StringComparison.OrdinalIgnoreCase);
        }

        public static void CollapseAll(this DTE2 dte, IEnumerable<string> exlusionList = null)
        {
            UIHierarchy solutionExplorer = dte.ToolWindows.SolutionExplorer;

            // Check if there is any open solution        
            if (solutionExplorer.UIHierarchyItems.Count == 0)
            {
                return;
            }

            // Get the top node (the name of the solution)        
            UIHierarchyItem rootNode = solutionExplorer.UIHierarchyItems.Item(1);
            rootNode.DTE.SuppressUI = true;

            // Collapse each project node        
            Collapse(rootNode, solutionExplorer, exlusionList);

            rootNode.Select(vsUISelectionType.vsUISelectionTypeSelect);
            rootNode.DTE.SuppressUI = false;
        }

        public static void Collapse(this UIHierarchyItem item, UIHierarchy solutionExplorer, IEnumerable<string> exlusionList = null)
        {
            foreach (UIHierarchyItem innerItem in item.UIHierarchyItems)
            {
                if (innerItem.UIHierarchyItems.Count > 0)
                {
                    // Recursive call                    
                    Collapse(innerItem, solutionExplorer);

                    // skip project folders
                    if (exlusionList != null && exlusionList.Any(e => e == innerItem.Name))
                    {
                        continue;
                    }

                    // Collapse                    
                    if (innerItem.UIHierarchyItems.Expanded)
                    {
                        innerItem.UIHierarchyItems.Expanded = false;
                        if (innerItem.UIHierarchyItems.Expanded)
                        {
                            innerItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
                            solutionExplorer.DoDefaultAction();
                        }
                    }
                }
            }
        }

        public static void CreateLib(this string directory, string[] resources, bool forceReplaceFiles = false)
        {
            directory = Path.Combine(directory, @"Lib");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var assembly = typeof(SolutionExtensions).Assembly;

            var bufferSize = 1024;
            var data = new byte[bufferSize];
            foreach (var resx in resources)
            {
                using (var stream = assembly.GetManifestResourceStream(string.Format("ProjectTemplateWizard.Resources.{0}", resx)))
                {
                    var file = Path.Combine(directory, resx);

                    if (forceReplaceFiles && File.Exists(file)) File.Delete(file);

                    if (!File.Exists(file))
                    {
                        using (var fileStream = new FileStream(file, FileMode.Create))
                        {
                            int count;
                            while ((count = stream.Read(data, 0, bufferSize)) > 0)
                            {
                                fileStream.Write(data, 0, count);
                            }

                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                    stream.Close();
                }
            }
        }

        public static void UnpackZipToLib(this string directory, string[] resources)
        {
            directory = Path.Combine(directory, @"Lib");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var assembly = typeof(SolutionExtensions).Assembly;

            var bufferSize = 1024;
            var data = new byte[bufferSize];
            foreach (var resx in resources)
            {
                using (var stream = assembly.GetManifestResourceStream(string.Format("ProjectTemplateWizard.Resources.{0}", resx)))
                {
                    using (var zipFile = new ZipFile(stream) { IsStreamOwner = true })
                    {
                        foreach (ZipEntry zipEntry in zipFile)
                        {
                            if (!zipEntry.IsFile) continue;

                            var buffer = new byte[4096];
                            var zipStream = zipFile.GetInputStream(zipEntry);

                            var targetFile = Path.Combine(directory, zipEntry.Name);
                            var targetDirectory = Path.GetDirectoryName(targetFile);
                            if (targetDirectory.Length > 0) Directory.CreateDirectory(targetDirectory);

                            using (var streamWriter = File.Create(targetFile))
                            {
                                StreamUtils.Copy(zipStream, streamWriter, buffer);
                            }
                        }

                        zipFile.Close();
                    }
                }
            }
        }

        public static bool IsParentDirectoryOf(this string directory, string directoryToCompare)
        {
            var d1 = new DirectoryInfo(directory).FullName;
            var d2 = new DirectoryInfo(directoryToCompare).FullName;
            return d2.Length > d1.Length && d2.StartsWith(d1);
        }
    }
}
