using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectTemplateWizard
{
    public partial class SilverlightLibWizard : Form
    {
        public string SolutionBaseNamespace { get; set; }

        public SilverlightLibWizard(string solutionBaseNamespace)
        {
            InitializeComponent();

            this.SolutionBaseNamespace = solutionBaseNamespace;

            this.baseNamespaceTextBox.TextChanged += (seder, e) =>
                {
                    var ns = this.baseNamespaceTextBox.Text;
                    this.clientLibraryNamespace.Text = string.Format("{0}.Client.Domain", ns);
                    this.modelNamespace.Text = string.Format("{0}.Common.Domain.Model", ns);
                    this.serviceContractsNamespace.Text = string.Format("{0}.Common.Domain.Service.Contracts", ns);
                };
        }

        protected override void OnShown(EventArgs e)
        {
            baseNamespaceTextBox.Text = SolutionBaseNamespace;

            base.OnShown(e);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SolutionBaseNamespace = baseNamespaceTextBox.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
