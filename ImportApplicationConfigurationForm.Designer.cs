namespace ZinvoiceTransformer
{
    partial class ImportApplicationConfigurationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportApplicationConfigurationForm));
            this._appLocationLabel = new System.Windows.Forms.Label();
            this._invoiceFileLocationLabel = new System.Windows.Forms.Label();
            this._mainPanel = new System.Windows.Forms.Panel();
            this._detailPanel = new System.Windows.Forms.Panel();
            this._setInvoiceFileLocationButton = new System.Windows.Forms.Button();
            this._setAppLocationButton = new System.Windows.Forms.Button();
            this._invoiceFileLocationTextBox = new System.Windows.Forms.TextBox();
            this._appLocationTextBox = new System.Windows.Forms.TextBox();
            this._buttonPanel = new System.Windows.Forms.Panel();
            this._saveButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this._mainPanel.SuspendLayout();
            this._detailPanel.SuspendLayout();
            this._buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _appLocationLabel
            // 
            this._appLocationLabel.AutoSize = true;
            this._appLocationLabel.Location = new System.Drawing.Point(12, 10);
            this._appLocationLabel.Name = "_appLocationLabel";
            this._appLocationLabel.Size = new System.Drawing.Size(102, 13);
            this._appLocationLabel.TabIndex = 0;
            this._appLocationLabel.Text = "Application location:";
            // 
            // _invoiceFileLocationLabel
            // 
            this._invoiceFileLocationLabel.AutoSize = true;
            this._invoiceFileLocationLabel.Location = new System.Drawing.Point(12, 61);
            this._invoiceFileLocationLabel.Name = "_invoiceFileLocationLabel";
            this._invoiceFileLocationLabel.Size = new System.Drawing.Size(101, 13);
            this._invoiceFileLocationLabel.TabIndex = 3;
            this._invoiceFileLocationLabel.Text = "Invoice file location:";
            // 
            // _mainPanel
            // 
            this._mainPanel.Controls.Add(this._detailPanel);
            this._mainPanel.Controls.Add(this._buttonPanel);
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Location = new System.Drawing.Point(0, 0);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(362, 171);
            this._mainPanel.TabIndex = 2;
            // 
            // _detailPanel
            // 
            this._detailPanel.Controls.Add(this._setInvoiceFileLocationButton);
            this._detailPanel.Controls.Add(this._setAppLocationButton);
            this._detailPanel.Controls.Add(this._invoiceFileLocationTextBox);
            this._detailPanel.Controls.Add(this._appLocationTextBox);
            this._detailPanel.Controls.Add(this._appLocationLabel);
            this._detailPanel.Controls.Add(this._invoiceFileLocationLabel);
            this._detailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detailPanel.Location = new System.Drawing.Point(0, 0);
            this._detailPanel.Name = "_detailPanel";
            this._detailPanel.Size = new System.Drawing.Size(362, 120);
            this._detailPanel.TabIndex = 0;
            // 
            // _setInvoiceFileLocationButton
            // 
            this._setInvoiceFileLocationButton.Location = new System.Drawing.Point(317, 74);
            this._setInvoiceFileLocationButton.Name = "_setInvoiceFileLocationButton";
            this._setInvoiceFileLocationButton.Size = new System.Drawing.Size(32, 23);
            this._setInvoiceFileLocationButton.TabIndex = 5;
            this._setInvoiceFileLocationButton.Text = "...";
            this._setInvoiceFileLocationButton.UseVisualStyleBackColor = true;
            // 
            // _setAppLocationButton
            // 
            this._setAppLocationButton.Location = new System.Drawing.Point(317, 26);
            this._setAppLocationButton.Name = "_setAppLocationButton";
            this._setAppLocationButton.Size = new System.Drawing.Size(32, 23);
            this._setAppLocationButton.TabIndex = 2;
            this._setAppLocationButton.Text = "...";
            this._setAppLocationButton.UseVisualStyleBackColor = true;
            // 
            // _invoiceFileLocationTextBox
            // 
            this._invoiceFileLocationTextBox.Location = new System.Drawing.Point(15, 77);
            this._invoiceFileLocationTextBox.Name = "_invoiceFileLocationTextBox";
            this._invoiceFileLocationTextBox.Size = new System.Drawing.Size(296, 20);
            this._invoiceFileLocationTextBox.TabIndex = 4;
            // 
            // _appLocationTextBox
            // 
            this._appLocationTextBox.Location = new System.Drawing.Point(15, 26);
            this._appLocationTextBox.Name = "_appLocationTextBox";
            this._appLocationTextBox.Size = new System.Drawing.Size(296, 20);
            this._appLocationTextBox.TabIndex = 1;
            // 
            // _buttonPanel
            // 
            this._buttonPanel.Controls.Add(this._saveButton);
            this._buttonPanel.Controls.Add(this._cancelButton);
            this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._buttonPanel.Location = new System.Drawing.Point(0, 120);
            this._buttonPanel.Name = "_buttonPanel";
            this._buttonPanel.Size = new System.Drawing.Size(362, 51);
            this._buttonPanel.TabIndex = 1;
            // 
            // _saveButton
            // 
            this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._saveButton.Location = new System.Drawing.Point(193, 16);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(75, 23);
            this._saveButton.TabIndex = 0;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(274, 16);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Close";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Exe files|*.exe|All files|*.*";
            this.openFileDialog1.InitialDirectory = "c:\\";
            this.openFileDialog1.Title = "File Import Application Location";
            // 
            // ImportApplicationConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 171);
            this.Controls.Add(this._mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportApplicationConfigurationForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Application Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportApplicationConfigurationForm_FormClosing);
            this._mainPanel.ResumeLayout(false);
            this._detailPanel.ResumeLayout(false);
            this._detailPanel.PerformLayout();
            this._buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _appLocationLabel;
        private System.Windows.Forms.Label _invoiceFileLocationLabel;
        private System.Windows.Forms.Panel _mainPanel;
        private System.Windows.Forms.Panel _detailPanel;
        private System.Windows.Forms.Panel _buttonPanel;
        private System.Windows.Forms.TextBox _invoiceFileLocationTextBox;
        private System.Windows.Forms.TextBox _appLocationTextBox;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _setInvoiceFileLocationButton;
        private System.Windows.Forms.Button _setAppLocationButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}