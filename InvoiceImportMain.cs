﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using LogThis;
using System.IO;
using System.Linq;
using zInvoiceTransformer.Comms;
using ZinvoiceTransformer.Properties;

namespace zInvoiceTransformer
{
    public partial class InvoiceImportMain : Form
    {
        string _errorMsg = "";
        string _infoMsg = "";
        private static InvoiceTemplateModel _invoiceTemplateModel;
        IClientTransferProtocol _clientTransferProtocol;

        public InvoiceImportMain()
        {
            InitializeComponent();
            InitialiseEvents();
            _invoiceTemplateModel = new InvoiceTemplateModel();
        }

        void InitialiseEvents()
        {
            _templateSelectorListBox.SelectedIndexChanged += OnSelectedTemplateChanged;
            browseLogFileFolderToolStripMenuItem.Click += OnBrowseLogFileFolderClick;
            templateEditorToolStripMenuItem.Click += OnOpenTemplateEditorClick;
            viewLogFileToolStripMenuItem.Click += OnViewLogFileClick;
            importApplicationSettingsToolStripMenuItem.Click += OnImportApSettingsClick;
            _doTransformButton.Click += OnDoTransformAndImportClick;
            _closeButton.Click += OnCloseClick;
            _getRemoteInvoicesButton.Click += OnGetRemoteInvoicesClick;
        }

        void InvoiceTransformer_Load(object sender, EventArgs e)
        {
            BindControls();
            LoadAndDisplayTemplates(null);
        }

        void BindControls()
        {
            _nameTextBox.DataBindings.Add(new Binding("Text", _invoiceTemplateModel, "SelectedTemplateName", false,
                DataSourceUpdateMode.Never));
            _descriptionTextBox.DataBindings.Add(new Binding("Text", _invoiceTemplateModel, "SelectedTemplateDescription",
                false, DataSourceUpdateMode.Never));
        }

        private void LoadAndDisplayTemplates(int? templateId)
        {
            _invoiceTemplateModel.LoadTemplates();
            _templateSelectorListBox.Items.Clear();
            _invoiceFilesListBox.Items.Clear();
            
            _templateSelectorListBox.Items.AddRange(GetListItemsForActiveTemplates());

            if (templateId == null)
                _templateSelectorListBox.SelectedIndex = 0;
            else
                _templateSelectorListBox.SelectedItem = _templateSelectorListBox.
                    Items.OfType<TemplateListItem>().
                    FirstOrDefault(ti => ti.Id == templateId.ToString());

            //_templateSelectorListBox.SetSelected(0, true);
        }

        private static TemplateListItem[] GetListItemsForActiveTemplates()
        {
            return _invoiceTemplateModel.GetAllTemplatesArray();
        }
                
        void OnSelectedTemplateChanged(object sender, EventArgs e)
        {
            if (_templateSelectorListBox.SelectedItem != null)
            {
                _invoiceTemplateModel.SetSelectedTemplate(Convert.ToByte(((TemplateListItem)_templateSelectorListBox.SelectedItem).Id));
                CheckWorkingFolders();
                RefreshFileList();
            }
        }

        static void StartZonalImportApp()
        {
            Log.LogThis("Starting Invoice Import Application", eloglevel.info);
            Log.LogThis($"Invoice Import Application location: '{_invoiceTemplateModel.ImportAppLocation}'", eloglevel.info);

            var invoiceImportProcess = new Process
            {
                StartInfo =
                {
                    UseShellExecute = true, 
                    FileName = _invoiceTemplateModel.ImportAppLocation
                }
            };

            invoiceImportProcess.Start();
            invoiceImportProcess.WaitForExit();
            Log.LogThis("Invoice Import Application closed", eloglevel.info);
        }

        private void TransformBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.LogThis("Starting Invoice transform process", eloglevel.info);
            _transformBackgroundWorker.ReportProgress(0);
            
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                // could return an info object with transform details instead of just an int
                var transformResultInfo = _invoiceTemplateModel.DoTransform();
                if (transformResultInfo.NumberOfInvoiceLinesProcessed > 0)
                {
                    _infoMsg = $"{transformResultInfo.NumberOfInvoiceLinesProcessed} invoice lines processed";
                    _transformBackgroundWorker.ReportProgress(25);
                }
                else
                {
                    _infoMsg = "No invoice lines found";
                    _transformBackgroundWorker.ReportProgress(0);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.LogThis($"An exception occurred during transform stage: {ex}", eloglevel.error);
                _errorMsg =  "An error occured performing the invoice transfrom";
                _transformBackgroundWorker.ReportProgress(0);
                return;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            try
            {
                _invoiceTemplateModel.IncreaseAlliantTablesImportRefField();
                _transformBackgroundWorker.ReportProgress(50);
            }
            catch (Exception ex)
            {
                Log.LogThis($"An exception occurred updating supplier name in Aztec: {ex}", eloglevel.error);
                _errorMsg = "An error occured updating suplier name in Aztec";
                _transformBackgroundWorker.ReportProgress(0);
                return;
            }

            try
            {
                _invoiceTemplateModel.UpdateInvoiceImportFieldDefinitions(_invoiceTemplateModel.SelectedTemplate);
                _transformBackgroundWorker.ReportProgress(75);
            }
            catch (Exception ex)
            {
                Log.LogThis($"An exception occurred writing field definitions to Aztec: {ex}", eloglevel.error);
                _errorMsg =  "An error occured writing field definitions to Aztec";
                _transformBackgroundWorker.ReportProgress(0);
                return;
            }

            try
            {
                _invoiceTemplateModel.UpdatePurSysVarImportFolder();
                _transformBackgroundWorker.ReportProgress(80);
            }
            catch (Exception ex)
            {
                Log.LogThis($"An exception occurred updating PurSysVar.ImportDir in Aztec: {ex}", eloglevel.error);
                _errorMsg = "An error occured updating PurSysVar.ImportDir in Aztec";
                _transformBackgroundWorker.ReportProgress(0);
                return;
            }

            try
            {
                StartZonalImportApp();
                _transformBackgroundWorker.ReportProgress(100);
            }
            catch (Exception ex)
            {
                Log.LogThis($"An exception occurred starting the Invoice Import application: {ex}", eloglevel.error);
                _errorMsg =  "An error occured while starting the Invoice Import application";
                _transformBackgroundWorker.ReportProgress(0);
                return;
            }
        }

        private void TransformBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _transformProgressBar.Value = e.ProgressPercentage;
        }

        private void TransformBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(_errorMsg))
            {
                Log.LogThis("Invoice transform process complete", eloglevel.info);
                MessageBox.Show(this, "Invoice import complete.\n" + _infoMsg, Resources.AppNameText, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, _errorMsg + ". See log file for details",
                                    Resources.AppNameText, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //LoadAndDisplayTemplates();
            RefreshFileList();
        }

        private void OnDoTransformAndImportClick(object sender, EventArgs e)
        {
            if (!CheckWorkingFolders())
            {
                MessageBox.Show(this, "Cannot run import.\nApplication folders are not valid", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _errorMsg = "";
            if (_invoiceTemplateModel.SelectedTemplate == null)
            {
                MessageBox.Show(this, "No invoice supplier selected.\nPlease select a supplier from the list.", Resources.AppNameText, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _transformBackgroundWorker.WorkerReportsProgress = true;
            _transformBackgroundWorker.RunWorkerAsync();
        }

        private static void OnViewLogFileClick(object sender, EventArgs e)
        {
            Log.LogThis("Loading active log file: " + Log.LogPath, eloglevel.info);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo("NotePad.exe", Log.LogPath)
            };
            p.Start();
        }

        private static void OnBrowseLogFileFolderClick(object sender, EventArgs e)
        {
            Log.LogThis("Loading log file folder: " + Path.GetDirectoryName(Log.LogPath), eloglevel.info);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo("explorer.exe", Path.GetDirectoryName(Log.LogPath))
            };
            p.Start();
        }

        private void OnOpenTemplateEditorClick(object sender, EventArgs e)
        {
            var templateEditor = new TemplateEditor(_invoiceTemplateModel);
            templateEditor.ShowDialog(this);
            LoadAndDisplayTemplates(_invoiceTemplateModel.SelectedTemplate.Id);
        }

        private void OnImportApSettingsClick(object sender, EventArgs e)
        {
            var importAppSettings = new ImportApplicationConfigurationForm(_invoiceTemplateModel);
            importAppSettings.ShowDialog(this);
        }

        private static void OnCloseClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnGetRemoteInvoicesClick(object sender, EventArgs e)
        {
            InitialiseClientConnectionDetails();
            if (_invoiceTemplateModel != null && 
                _clientTransferProtocol != null &&
                _clientTransferProtocol.RemoteConnectionInfo != null)
            {
                var fileDownloadDialog = new RemoteDownloadDialog(_invoiceTemplateModel, _clientTransferProtocol);
                fileDownloadDialog.ShowDialog(this);
                RefreshFileList();
            }
        }

        private void RefreshFileList()
        {
            _invoiceFilesListBox.Items.Clear();

            var fileList = _invoiceTemplateModel.GetSelectedTemplateImportFiles();

            if (fileList != null)
                _invoiceFilesListBox.Items.AddRange(fileList);
        }

        bool CheckWorkingFolders()
        {
            var result = true;

            if (!Directory.Exists(_invoiceTemplateModel.SelectedTemplate.SourceFolder))
            {
                if (ShowFolderNotFoundDialog("source", _invoiceTemplateModel.SelectedTemplate.SourceFolder) ==
                    DialogResult.Yes)
                {
                    Directory.CreateDirectory(_invoiceTemplateModel.SelectedTemplate.SourceFolder);
                }
                else
                {
                    result = false;
                }
            }

            if (!Directory.Exists(_invoiceTemplateModel.SelectedTemplate.OutputFolder))
            {
                if (ShowFolderNotFoundDialog("output", _invoiceTemplateModel.SelectedTemplate.OutputFolder) ==
                    DialogResult.Yes)
                {
                    Directory.CreateDirectory(_invoiceTemplateModel.SelectedTemplate.OutputFolder);
                }
                else
                {
                    result = false;
                }
            }

            if (!File.Exists(_invoiceTemplateModel.ImportAppLocation))
            {
                MessageBox.Show(this,
                    "The Import Application cannot be found.\nCheck 'Tools | Application Settings'",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                
                result = false;
            }

            return result;
        }

        DialogResult ShowFolderNotFoundDialog(string folderType, string folderPath)
        {
            return 
                MessageBox.Show(
                this,
                $"Could not find invoice {folderType} folder: " + 
                '\n' + folderPath + "\n\nDo you want to create the folder now?",
                Resources.AppNameText,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);
        }

        void InitialiseClientConnectionDetails()
        {
            if (_invoiceTemplateModel == null)
            {
                MessageBox.Show(this,
                    "The selected provider does not have a template definition. Check template settings.",
                    "Invoice Template Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            _clientTransferProtocol = RemoteConnectionFactory.Build(
                Convert.ToInt32(_invoiceTemplateModel.SelectedTemplate
                                    ?.RemoteInvoiceSettings
                                    ?.RemoteTransferProtocolTypeId));

            if (_clientTransferProtocol == null)
            {
                MessageBox.Show(this,
                    "The selected provider has not been configured for remote invoice downloads. Check template settings.",
                    "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _clientTransferProtocol.RemoteConnectionInfo = _invoiceTemplateModel.GetSelectedTemplateConnectionInfo();
            if (_clientTransferProtocol.RemoteConnectionInfo == null)
            {
                MessageBox.Show(this,
                    "Cannot find connection details for the selected provider. Check template settings.",
                    "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //InitialiseClientConnectionDetails();
            //if (_clientTransferProtocol.CheckConnection())
            //{
            //    _clientTransferProtocol.UploadFile("ZonalInvoiceImport.exe_20201106.log");
            //}
        }
    }
}
