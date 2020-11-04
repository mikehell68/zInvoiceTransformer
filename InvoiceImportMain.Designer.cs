namespace ZinvoiceTransformer
{
    partial class InvoiceImportMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InvoiceImportMain));
            this._doTransformButton = new System.Windows.Forms.Button();
            this._templateSelectorListBox = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this._transformProgressBar = new System.Windows.Forms.ProgressBar();
            this._transformBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._progressPanel = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._closeButton = new System.Windows.Forms.Button();
            this._invoiceFilesListBox = new System.Windows.Forms.ListBox();
            this.label8 = new System.Windows.Forms.Label();
            this._mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browseLogFileFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importApplicationSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.templateEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mainPanel = new System.Windows.Forms.Panel();
            this._contentPanel = new System.Windows.Forms.Panel();
            this._templateInfoPanel = new System.Windows.Forms.Panel();
            this._templateInfoInnerPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this._descriptionTextBox = new System.Windows.Forms.TextBox();
            this._nameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._templateListPanel = new System.Windows.Forms.Panel();
            this._buttonPanel = new System.Windows.Forms.Panel();
            this._getRemoteInvoicesButton = new System.Windows.Forms.Button();
            this._progressPanel.SuspendLayout();
            this._mainMenuStrip.SuspendLayout();
            this._mainPanel.SuspendLayout();
            this._contentPanel.SuspendLayout();
            this._templateInfoPanel.SuspendLayout();
            this._templateInfoInnerPanel.SuspendLayout();
            this._templateListPanel.SuspendLayout();
            this._buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _doTransformButton
            // 
            this._doTransformButton.Location = new System.Drawing.Point(275, 11);
            this._doTransformButton.Name = "_doTransformButton";
            this._doTransformButton.Size = new System.Drawing.Size(133, 23);
            this._doTransformButton.TabIndex = 7;
            this._doTransformButton.Text = "Do Import";
            this._doTransformButton.UseVisualStyleBackColor = true;
            // 
            // _templateSelectorListBox
            // 
            this._templateSelectorListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._templateSelectorListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._templateSelectorListBox.FormattingEnabled = true;
            this._templateSelectorListBox.Location = new System.Drawing.Point(0, 21);
            this._templateSelectorListBox.Name = "_templateSelectorListBox";
            this._templateSelectorListBox.Size = new System.Drawing.Size(198, 271);
            this._templateSelectorListBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this._templateSelectorListBox, "The selected the invoice template will be used during the transform");
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(198, 21);
            this.label5.TabIndex = 11;
            this.label5.Text = "Suppliers";
            // 
            // _transformProgressBar
            // 
            this._transformProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this._transformProgressBar.Location = new System.Drawing.Point(0, 0);
            this._transformProgressBar.Name = "_transformProgressBar";
            this._transformProgressBar.Size = new System.Drawing.Size(559, 18);
            this._transformProgressBar.TabIndex = 12;
            // 
            // _transformBackgroundWorker
            // 
            this._transformBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._transformBackgroundWorker_DoWork);
            this._transformBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._transformBackgroundWorker_ProgressChanged);
            this._transformBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._transformBackgroundWorker_RunWorkerCompleted);
            // 
            // _progressPanel
            // 
            this._progressPanel.Controls.Add(this._transformProgressBar);
            this._progressPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._progressPanel.Location = new System.Drawing.Point(0, 362);
            this._progressPanel.Name = "_progressPanel";
            this._progressPanel.Size = new System.Drawing.Size(559, 18);
            this._progressPanel.TabIndex = 13;
            // 
            // _closeButton
            // 
            this._closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._closeButton.Location = new System.Drawing.Point(414, 11);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(133, 23);
            this._closeButton.TabIndex = 8;
            this._closeButton.Text = "Close";
            this._closeButton.UseVisualStyleBackColor = true;
            // 
            // _invoiceFilesListBox
            // 
            this._invoiceFilesListBox.FormattingEnabled = true;
            this._invoiceFilesListBox.HorizontalScrollbar = true;
            this._invoiceFilesListBox.Location = new System.Drawing.Point(6, 137);
            this._invoiceFilesListBox.Name = "_invoiceFilesListBox";
            this._invoiceFilesListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this._invoiceFilesListBox.Size = new System.Drawing.Size(341, 121);
            this._invoiceFilesListBox.TabIndex = 6;
            this._invoiceFilesListBox.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 121);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(190, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "The invoice files below will be imported";
            // 
            // _mainMenuStrip
            // 
            this._mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this._mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this._mainMenuStrip.Name = "_mainMenuStrip";
            this._mainMenuStrip.Size = new System.Drawing.Size(559, 24);
            this._mainMenuStrip.TabIndex = 2;
            this._mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Close";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewLogFileToolStripMenuItem,
            this.browseLogFileFolderToolStripMenuItem,
            this.toolStripSeparator2,
            this.importApplicationSettingsToolStripMenuItem,
            this.templateEditorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // viewLogFileToolStripMenuItem
            // 
            this.viewLogFileToolStripMenuItem.Name = "viewLogFileToolStripMenuItem";
            this.viewLogFileToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.viewLogFileToolStripMenuItem.Text = "View Log File";
            // 
            // browseLogFileFolderToolStripMenuItem
            // 
            this.browseLogFileFolderToolStripMenuItem.Name = "browseLogFileFolderToolStripMenuItem";
            this.browseLogFileFolderToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.browseLogFileFolderToolStripMenuItem.Text = "Browse Log File Folder";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(189, 6);
            // 
            // importApplicationSettingsToolStripMenuItem
            // 
            this.importApplicationSettingsToolStripMenuItem.Name = "importApplicationSettingsToolStripMenuItem";
            this.importApplicationSettingsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.importApplicationSettingsToolStripMenuItem.Text = "Application Settings";
            // 
            // templateEditorToolStripMenuItem
            // 
            this.templateEditorToolStripMenuItem.Name = "templateEditorToolStripMenuItem";
            this.templateEditorToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.templateEditorToolStripMenuItem.Text = "Template Editor";
            // 
            // _mainPanel
            // 
            this._mainPanel.Controls.Add(this._contentPanel);
            this._mainPanel.Controls.Add(this._buttonPanel);
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Location = new System.Drawing.Point(0, 24);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(559, 338);
            this._mainPanel.TabIndex = 0;
            // 
            // _contentPanel
            // 
            this._contentPanel.Controls.Add(this._templateInfoPanel);
            this._contentPanel.Controls.Add(this._templateListPanel);
            this._contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._contentPanel.Location = new System.Drawing.Point(0, 0);
            this._contentPanel.Name = "_contentPanel";
            this._contentPanel.Size = new System.Drawing.Size(559, 294);
            this._contentPanel.TabIndex = 18;
            // 
            // _templateInfoPanel
            // 
            this._templateInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._templateInfoPanel.Controls.Add(this._templateInfoInnerPanel);
            this._templateInfoPanel.Controls.Add(this.label3);
            this._templateInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._templateInfoPanel.Location = new System.Drawing.Point(200, 0);
            this._templateInfoPanel.Name = "_templateInfoPanel";
            this._templateInfoPanel.Size = new System.Drawing.Size(359, 294);
            this._templateInfoPanel.TabIndex = 17;
            // 
            // _templateInfoInnerPanel
            // 
            this._templateInfoInnerPanel.Controls.Add(this.label1);
            this._templateInfoInnerPanel.Controls.Add(this._descriptionTextBox);
            this._templateInfoInnerPanel.Controls.Add(this._invoiceFilesListBox);
            this._templateInfoInnerPanel.Controls.Add(this._nameTextBox);
            this._templateInfoInnerPanel.Controls.Add(this.label8);
            this._templateInfoInnerPanel.Controls.Add(this.label2);
            this._templateInfoInnerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._templateInfoInnerPanel.Location = new System.Drawing.Point(0, 21);
            this._templateInfoInnerPanel.Name = "_templateInfoInnerPanel";
            this._templateInfoInnerPanel.Size = new System.Drawing.Size(357, 271);
            this._templateInfoInnerPanel.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name: ";
            // 
            // _descriptionTextBox
            // 
            this._descriptionTextBox.Location = new System.Drawing.Point(6, 68);
            this._descriptionTextBox.Multiline = true;
            this._descriptionTextBox.Name = "_descriptionTextBox";
            this._descriptionTextBox.ReadOnly = true;
            this._descriptionTextBox.Size = new System.Drawing.Size(341, 41);
            this._descriptionTextBox.TabIndex = 2;
            // 
            // _nameTextBox
            // 
            this._nameTextBox.Location = new System.Drawing.Point(6, 29);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.ReadOnly = true;
            this._nameTextBox.Size = new System.Drawing.Size(341, 20);
            this._nameTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Description";
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(357, 21);
            this.label3.TabIndex = 18;
            this.label3.Text = "Supplier Details";
            // 
            // _templateListPanel
            // 
            this._templateListPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._templateListPanel.Controls.Add(this._templateSelectorListBox);
            this._templateListPanel.Controls.Add(this.label5);
            this._templateListPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this._templateListPanel.Location = new System.Drawing.Point(0, 0);
            this._templateListPanel.Name = "_templateListPanel";
            this._templateListPanel.Size = new System.Drawing.Size(200, 294);
            this._templateListPanel.TabIndex = 18;
            // 
            // _buttonPanel
            // 
            this._buttonPanel.Controls.Add(this._getRemoteInvoicesButton);
            this._buttonPanel.Controls.Add(this._doTransformButton);
            this._buttonPanel.Controls.Add(this._closeButton);
            this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._buttonPanel.Location = new System.Drawing.Point(0, 294);
            this._buttonPanel.Name = "_buttonPanel";
            this._buttonPanel.Size = new System.Drawing.Size(559, 44);
            this._buttonPanel.TabIndex = 17;
            // 
            // _getRemoteInvoicesButton
            // 
            this._getRemoteInvoicesButton.Location = new System.Drawing.Point(12, 11);
            this._getRemoteInvoicesButton.Name = "_getRemoteInvoicesButton";
            this._getRemoteInvoicesButton.Size = new System.Drawing.Size(150, 23);
            this._getRemoteInvoicesButton.TabIndex = 9;
            this._getRemoteInvoicesButton.Text = "Download Remote Invoices";
            this._getRemoteInvoicesButton.UseVisualStyleBackColor = true;
            // 
            // InvoiceImportMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._closeButton;
            this.ClientSize = new System.Drawing.Size(559, 380);
            this.Controls.Add(this._mainPanel);
            this.Controls.Add(this._progressPanel);
            this.Controls.Add(this._mainMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this._mainMenuStrip;
            this.Name = "InvoiceImportMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zonal Invoice Import";
            this.Load += new System.EventHandler(this.InvoiceTransformer_Load);
            this._progressPanel.ResumeLayout(false);
            this._mainMenuStrip.ResumeLayout(false);
            this._mainMenuStrip.PerformLayout();
            this._mainPanel.ResumeLayout(false);
            this._contentPanel.ResumeLayout(false);
            this._templateInfoPanel.ResumeLayout(false);
            this._templateInfoInnerPanel.ResumeLayout(false);
            this._templateInfoInnerPanel.PerformLayout();
            this._templateListPanel.ResumeLayout(false);
            this._buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _doTransformButton;
        private System.Windows.Forms.ListBox _templateSelectorListBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar _transformProgressBar;
        private System.ComponentModel.BackgroundWorker _transformBackgroundWorker;
        private System.Windows.Forms.Panel _progressPanel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button _closeButton;
        private System.Windows.Forms.ListBox _invoiceFilesListBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MenuStrip _mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseLogFileFolderToolStripMenuItem;
        private System.Windows.Forms.Panel _mainPanel;
        private System.Windows.Forms.ToolStripMenuItem templateEditorToolStripMenuItem;
        private System.Windows.Forms.Panel _templateInfoPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem importApplicationSettingsToolStripMenuItem;
        private System.Windows.Forms.TextBox _descriptionTextBox;
        private System.Windows.Forms.TextBox _nameTextBox;
        private System.Windows.Forms.Panel _contentPanel;
        private System.Windows.Forms.Panel _templateListPanel;
        private System.Windows.Forms.Panel _buttonPanel;
        private System.Windows.Forms.Panel _templateInfoInnerPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Button _getRemoteInvoicesButton;
    }
}