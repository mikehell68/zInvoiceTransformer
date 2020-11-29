using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using LogThis;
using System.IO;
using System.Xml.Serialization;
using zInvoiceTransformer.Comms;
using ZinvoiceTransformer.Properties;
using ZinvoiceTransformer.XmlHelpers;
using ZinvoiceTransformer.XmlModels;

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

            //string xml = File.ReadAllText( InvoiceTemplateModel.InvoiceImportTemplatePath);
            //_importTemplates = xml.ParseXml<InvoiceImportTemplates>();
            //_importTemplates.Templates.FirstOrDefault(t => t.Id == 2).RemoteInvoiceSettings =
            //    new InvoiceImportTemplatesTemplateRemoteInvoiceSettings
            //    {
            //        InvoiceFileCustomerPrefix = "", 
            //        keyfileLocation = "", 
            //        password = "xyz", 
            //        port = 22,
            //        RemoteFolder = "/inv", 
            //        RemoteTransferProtocolTypeId = 2, 
            //        url = "", 
            //        username = "mike"
            //    };
            
            //XmlSerializer s = new XmlSerializer(typeof(InvoiceImportTemplates));
            //FileStream file = File.Create(InvoiceTemplateModel.InvoiceImportTemplatePath +"." +DateTime.Now.TimeOfDay.Ticks);
            //s.Serialize(file, _importTemplates);
            //file.Close();
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
            LoadAndDisplayTemplates();
        }

        void BindControls()
        {
            _nameTextBox.DataBindings.Add(new Binding("Text", _invoiceTemplateModel, "SelectedTemplateName", false,
                DataSourceUpdateMode.Never));
            _descriptionTextBox.DataBindings.Add(new Binding("Text", _invoiceTemplateModel, "SelectedTemplateDescription",
                false, DataSourceUpdateMode.Never));
        }

        private void LoadAndDisplayTemplates()
        {
            _invoiceTemplateModel.LoadTemplates();
            _templateSelectorListBox.Items.Clear();
            _invoiceFilesListBox.Items.Clear();
            
            _templateSelectorListBox.Items.AddRange(GetListItemsForActiveTemplates());
            _templateSelectorListBox.SetSelected(0, true);
        }

        private static TemplateListItem[] GetListItemsForActiveTemplates()
        {
            return _invoiceTemplateModel.GetAllTemplatesArray();
            //return
            //    _invoiceTemplateModel.GetAllActiveTemplates().Select(
            //        template => new TemplateListItem
            //            {
            //                Id = template.Attribute("Id").Value,
            //                Name = template.Attribute("Name").Value,
            //                IsInUse = template.Attribute("Active").Value == "1"
            //            }).ToArray();
        }
                
        void OnSelectedTemplateChanged(object sender, EventArgs e)
        {
            if (_templateSelectorListBox.SelectedItem != null)
            {
                //_invoiceTemplateModel.SelectedTemplate = _invoiceTemplateModel.GetTemplate(((TemplateListItem)_templateSelectorListBox.SelectedItem).Id);
                _invoiceTemplateModel.SetSelectedTemplate(Convert.ToByte(((TemplateListItem)_templateSelectorListBox.SelectedItem).Id));
                _invoiceFilesListBox.Items.Clear();

                var fileList = _invoiceTemplateModel.GetSelectedTemplateImportFiles();

                if (fileList != null)
                    _invoiceFilesListBox.Items.AddRange(fileList);
                else
                {
                    if( MessageBox.Show(
                            this, 
                            "Could not find invoice source folder: " + '\n' +
                            _invoiceTemplateModel.SelectedTemplate.SourceFolder + 
                            '\n' + "Do you want to create the folder now?",
                            Resources.AppNameText, 
                            MessageBoxButtons.YesNoCancel, 
                            MessageBoxIcon.Warning)  == DialogResult.Yes )
                    {
                        Directory.CreateDirectory(_invoiceTemplateModel.SelectedTemplate.SourceFolder);
                    }
                }
            }
        }

        static void StartZonalImportApp()
        {
            Log.LogThis("Starting Invoice Import Appliction", eloglevel.info);

            var invoiceImportProcess = new Process();

            invoiceImportProcess.StartInfo.UseShellExecute = true;
            invoiceImportProcess.StartInfo.FileName = _invoiceTemplateModel.ImportAppLocation;
            invoiceImportProcess.Start();
            invoiceImportProcess.WaitForExit();
            Log.LogThis("Invoice Import Appliction closed", eloglevel.info);
        }

        private void TransformBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.LogThis("Starting Invoice transform process", eloglevel.info);
            _transformBackgroundWorker.ReportProgress(0);
            
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                // could return an info object with transform details instead of just an int
                TransformResultInfo transformResultInfo = _invoiceTemplateModel.DoTransform();
                if (transformResultInfo.NumberOfInvoiceLinesProcessed > 0)
                {
                    _infoMsg = string.Format("{0} invoice lines processed", transformResultInfo.NumberOfInvoiceLinesProcessed);
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
                Log.LogThis(string.Format("An exception occurred during transform stage: {0}", ex), eloglevel.error);
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
                Log.LogThis(string.Format("An exception occurred updating supplier name in Aztec: {0}", ex), eloglevel.error);
                _errorMsg = "An error occured updating suplier name in Aztec";
                _transformBackgroundWorker.ReportProgress(0);
                return;
            }

            try
            {
                _invoiceTemplateModel.UpdateInvoiceImportFieldDefinitions(_invoiceTemplateModel.SelectedTemplate);
                //WriteTransformValuesToAztec(_invoiceTemplateModel.SelectedTemplate);
                _transformBackgroundWorker.ReportProgress(75);
            }
            catch (Exception ex)
            {
                Log.LogThis(string.Format("An exception occurred writing field definitions to Aztec: {0}", ex), eloglevel.error);
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
                Log.LogThis(string.Format("An exception occurred updating PurSysVar.ImportDir in Aztec: {0}", ex), eloglevel.error);
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
                Log.LogThis(string.Format("An exception occurred starting the Invoice Import application: {0}", ex), eloglevel.error);
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

            LoadAndDisplayTemplates();
        }

        private void OnDoTransformAndImportClick(object sender, EventArgs e)
        {
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
            LoadAndDisplayTemplates();
        }

        private void OnImportApSettingsClick(object sender, EventArgs e)
        {
            var importAppSettings = new ImportApplicationConfigurationForm(_invoiceTemplateModel);
            importAppSettings.ShowDialog(this);
            //LoadAndDisplayTemplates();
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
            }
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
            InitialiseClientConnectionDetails();
            if (_clientTransferProtocol.CheckConnection())
            {
                _clientTransferProtocol.UploadFile("ZonalInvoiceImport.exe_20201106.log");
            }
        }
    }
}
