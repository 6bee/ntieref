namespace ProjectTemplateWizard
{
    partial class SilverlightLibWizard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SilverlightLibWizard));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.serviceContractsNamespace = new System.Windows.Forms.Label();
            this.modelNamespace = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.clientLibraryNamespace = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.baseNamespaceTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.serviceContractsNamespace);
            this.groupBox1.Controls.Add(this.modelNamespace);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.clientLibraryNamespace);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.baseNamespaceTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(393, 217);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Namespaces";
            // 
            // serviceContractsNamespace
            // 
            this.serviceContractsNamespace.AutoSize = true;
            this.serviceContractsNamespace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serviceContractsNamespace.Location = new System.Drawing.Point(6, 187);
            this.serviceContractsNamespace.Name = "serviceContractsNamespace";
            this.serviceContractsNamespace.Size = new System.Drawing.Size(39, 13);
            this.serviceContractsNamespace.TabIndex = 7;
            this.serviceContractsNamespace.Text = "A.B.C";
            // 
            // modelNamespace
            // 
            this.modelNamespace.AutoSize = true;
            this.modelNamespace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modelNamespace.Location = new System.Drawing.Point(6, 144);
            this.modelNamespace.Name = "modelNamespace";
            this.modelNamespace.Size = new System.Drawing.Size(39, 13);
            this.modelNamespace.TabIndex = 6;
            this.modelNamespace.Text = "A.B.C";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 174);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(151, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Service contracts namespace:";
            // 
            // clientLibraryNamespace
            // 
            this.clientLibraryNamespace.AccessibleRole = System.Windows.Forms.AccessibleRole.Cursor;
            this.clientLibraryNamespace.AutoSize = true;
            this.clientLibraryNamespace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clientLibraryNamespace.Location = new System.Drawing.Point(6, 104);
            this.clientLibraryNamespace.Name = "clientLibraryNamespace";
            this.clientLibraryNamespace.Size = new System.Drawing.Size(39, 13);
            this.clientLibraryNamespace.TabIndex = 4;
            this.clientLibraryNamespace.Text = "A.B.C";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Model namespace:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Client library namespace:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Base namespace:";
            // 
            // baseNamespaceTextBox
            // 
            this.baseNamespaceTextBox.Location = new System.Drawing.Point(9, 45);
            this.baseNamespaceTextBox.Name = "baseNamespaceTextBox";
            this.baseNamespaceTextBox.Size = new System.Drawing.Size(378, 20);
            this.baseNamespaceTextBox.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(330, 244);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(249, 244);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SilverlightLibWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 279);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SilverlightLibWizard";
            this.Text = "N-Tier Entity Framework Silverlight Library";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label serviceContractsNamespace;
        private System.Windows.Forms.Label modelNamespace;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label clientLibraryNamespace;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox baseNamespaceTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}