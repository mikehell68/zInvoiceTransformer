﻿namespace zInvoiceTransformer
{
    partial class RemoteDownloadDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteDownloadDialog));
            this._mainPanel = new System.Windows.Forms.Panel();
            this._fileListPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this._selectAllCheckBox = new System.Windows.Forms.CheckBox();
            this._filesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this._infoPanel = new System.Windows.Forms.Panel();
            this._portLabel = new System.Windows.Forms.Label();
            this._hostLabel = new System.Windows.Forms.Label();
            this._destinationLabel = new System.Windows.Forms.Label();
            this._remoteLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._buttonPanel = new System.Windows.Forms.Panel();
            this._closeFormButton = new System.Windows.Forms.Button();
            this._downloadFilesButton = new System.Windows.Forms.Button();
            this._refreshListButton = new System.Windows.Forms.Button();
            this._getFilesBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this._mainPanel.SuspendLayout();
            this._fileListPanel.SuspendLayout();
            this._infoPanel.SuspendLayout();
            this._buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mainPanel
            // 
            this._mainPanel.Controls.Add(this._fileListPanel);
            this._mainPanel.Controls.Add(this._infoPanel);
            this._mainPanel.Controls.Add(this._buttonPanel);
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Location = new System.Drawing.Point(0, 0);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(551, 554);
            this._mainPanel.TabIndex = 0;
            // 
            // _fileListPanel
            // 
            this._fileListPanel.Controls.Add(this.progressBar1);
            this._fileListPanel.Controls.Add(this.label1);
            this._fileListPanel.Controls.Add(this._selectAllCheckBox);
            this._fileListPanel.Controls.Add(this._filesCheckedListBox);
            this._fileListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._fileListPanel.Location = new System.Drawing.Point(0, 100);
            this._fileListPanel.Name = "_fileListPanel";
            this._fileListPanel.Size = new System.Drawing.Size(551, 377);
            this._fileListPanel.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Files available for download:";
            // 
            // _selectAllCheckBox
            // 
            this._selectAllCheckBox.AutoSize = true;
            this._selectAllCheckBox.Location = new System.Drawing.Point(14, 44);
            this._selectAllCheckBox.Name = "_selectAllCheckBox";
            this._selectAllCheckBox.Size = new System.Drawing.Size(168, 24);
            this._selectAllCheckBox.TabIndex = 1;
            this._selectAllCheckBox.Text = "Select/Deselect All";
            this._selectAllCheckBox.UseVisualStyleBackColor = true;
            // 
            // _filesCheckedListBox
            // 
            this._filesCheckedListBox.FormattingEnabled = true;
            this._filesCheckedListBox.Location = new System.Drawing.Point(12, 74);
            this._filesCheckedListBox.Name = "_filesCheckedListBox";
            this._filesCheckedListBox.Size = new System.Drawing.Size(527, 280);
            this._filesCheckedListBox.TabIndex = 0;
            // 
            // _infoPanel
            // 
            this._infoPanel.Controls.Add(this._portLabel);
            this._infoPanel.Controls.Add(this._hostLabel);
            this._infoPanel.Controls.Add(this._destinationLabel);
            this._infoPanel.Controls.Add(this._remoteLabel);
            this._infoPanel.Controls.Add(this.label5);
            this._infoPanel.Controls.Add(this.label4);
            this._infoPanel.Controls.Add(this.label3);
            this._infoPanel.Controls.Add(this.label2);
            this._infoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._infoPanel.Location = new System.Drawing.Point(0, 0);
            this._infoPanel.Name = "_infoPanel";
            this._infoPanel.Size = new System.Drawing.Size(551, 100);
            this._infoPanel.TabIndex = 1;
            // 
            // _portLabel
            // 
            this._portLabel.AutoSize = true;
            this._portLabel.Location = new System.Drawing.Point(472, 9);
            this._portLabel.Name = "_portLabel";
            this._portLabel.Size = new System.Drawing.Size(42, 20);
            this._portLabel.TabIndex = 11;
            this._portLabel.Text = "Port:";
            // 
            // _hostLabel
            // 
            this._hostLabel.AutoSize = true;
            this._hostLabel.Location = new System.Drawing.Point(65, 9);
            this._hostLabel.Name = "_hostLabel";
            this._hostLabel.Size = new System.Drawing.Size(47, 20);
            this._hostLabel.TabIndex = 10;
            this._hostLabel.Text = "Host:";
            // 
            // _destinationLabel
            // 
            this._destinationLabel.AutoSize = true;
            this._destinationLabel.Location = new System.Drawing.Point(164, 68);
            this._destinationLabel.Name = "_destinationLabel";
            this._destinationLabel.Size = new System.Drawing.Size(138, 20);
            this._destinationLabel.TabIndex = 9;
            this._destinationLabel.Text = "Destination folder:";
            // 
            // _remoteLabel
            // 
            this._remoteLabel.AutoSize = true;
            this._remoteLabel.Location = new System.Drawing.Point(164, 38);
            this._remoteLabel.Name = "_remoteLabel";
            this._remoteLabel.Size = new System.Drawing.Size(114, 20);
            this._remoteLabel.TabIndex = 8;
            this._remoteLabel.Text = "Remote folder:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "Destination folder:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Remote folder:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(424, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Host:";
            // 
            // _buttonPanel
            // 
            this._buttonPanel.Controls.Add(this._closeFormButton);
            this._buttonPanel.Controls.Add(this._downloadFilesButton);
            this._buttonPanel.Controls.Add(this._refreshListButton);
            this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._buttonPanel.Location = new System.Drawing.Point(0, 477);
            this._buttonPanel.Name = "_buttonPanel";
            this._buttonPanel.Size = new System.Drawing.Size(551, 77);
            this._buttonPanel.TabIndex = 0;
            // 
            // _closeFormButton
            // 
            this._closeFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._closeFormButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._closeFormButton.Location = new System.Drawing.Point(428, 6);
            this._closeFormButton.Name = "_closeFormButton";
            this._closeFormButton.Size = new System.Drawing.Size(111, 54);
            this._closeFormButton.TabIndex = 1;
            this._closeFormButton.Text = "Close";
            this._closeFormButton.UseVisualStyleBackColor = true;
            // 
            // _downloadFilesButton
            // 
            this._downloadFilesButton.Location = new System.Drawing.Point(253, 6);
            this._downloadFilesButton.Name = "_downloadFilesButton";
            this._downloadFilesButton.Size = new System.Drawing.Size(169, 54);
            this._downloadFilesButton.TabIndex = 0;
            this._downloadFilesButton.Text = "Download Files";
            this._downloadFilesButton.UseVisualStyleBackColor = true;
            // 
            // _refreshListButton
            // 
            this._refreshListButton.Location = new System.Drawing.Point(7, 25);
            this._refreshListButton.Name = "_refreshListButton";
            this._refreshListButton.Size = new System.Drawing.Size(96, 35);
            this._refreshListButton.TabIndex = 3;
            this._refreshListButton.Text = "Refresh";
            this._refreshListButton.UseVisualStyleBackColor = true;
            this._refreshListButton.Click += new System.EventHandler(this._refreshListButton_Click);
            // 
            // _getFilesBackgroundWorker
            // 
            this._getFilesBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._getFilesBackgroundWorker_DoWork);
            this._getFilesBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._getFilesBackgroundWorker_ProgressChanged);
            this._getFilesBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._getFilesBackgroundWorker_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 360);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(527, 11);
            this.progressBar1.TabIndex = 3;
            // 
            // RemoteDownloadDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._closeFormButton;
            this.ClientSize = new System.Drawing.Size(551, 554);
            this.Controls.Add(this._mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemoteDownloadDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Remote Download";
            this.Shown += new System.EventHandler(this.RemoteDownloadDialog_Shown);
            this._mainPanel.ResumeLayout(false);
            this._fileListPanel.ResumeLayout(false);
            this._fileListPanel.PerformLayout();
            this._infoPanel.ResumeLayout(false);
            this._infoPanel.PerformLayout();
            this._buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _mainPanel;
        private System.Windows.Forms.Panel _fileListPanel;
        private System.Windows.Forms.CheckedListBox _filesCheckedListBox;
        private System.Windows.Forms.Panel _infoPanel;
        private System.Windows.Forms.Panel _buttonPanel;
        private System.Windows.Forms.Button _closeFormButton;
        private System.Windows.Forms.Button _downloadFilesButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox _selectAllCheckBox;
        private System.Windows.Forms.Button _refreshListButton;
        private System.Windows.Forms.Label _portLabel;
        private System.Windows.Forms.Label _hostLabel;
        private System.Windows.Forms.Label _destinationLabel;
        private System.Windows.Forms.Label _remoteLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker _getFilesBackgroundWorker;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}