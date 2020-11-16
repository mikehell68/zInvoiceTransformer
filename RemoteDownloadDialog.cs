using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using zInvoiceTransformer.Comms;

namespace zInvoiceTransformer
{
    public partial class RemoteDownloadDialog : Form
    {
        IClientTransferProtocol _clientTransferProtocol;
        readonly InvoiceTemplateModel _invoiceTemplateModel;
        private List<string> _fileList;

        public RemoteDownloadDialog(InvoiceTemplateModel invoiceTemplateModel)
        {
            InitializeComponent();
            _invoiceTemplateModel = invoiceTemplateModel;
            _getFilesBackgroundWorker.WorkerReportsProgress = true;
            InitialiseClientConnectionDetails();
        }

        void InitialiseClientConnectionDetails()
        {
            if(_invoiceTemplateModel != null)
                _clientTransferProtocol = RemoteConnectionFactory.Build(
                    Convert.ToInt32(_invoiceTemplateModel.SelectedTemplate
                        ?.Element("RemoteInvoiceSettings")
                        ?.Attribute("RemoteTransferProtocolTypeId")
                        ?.Value ?? "0"));
        }

        void RemoteDownloadDialog_Shown(object sender, EventArgs e)
        {
            StartGetFileListBackgroundWorker();
        }

        void StartGetFileListBackgroundWorker()
        {
            if (_getFilesBackgroundWorker.IsBusy)
                return;

            ClearFileList();
            SetUiProgressState(0, true, false);

            _getFilesBackgroundWorker.RunWorkerAsync();
        }

        void ClearFileList()
        {
            _selectAllCheckBox.Checked = false;
            _filesCheckedListBox.Items.Clear();
            _fileList?.Clear();
        }

        void ConnectAndGetFiles()
        {
            _getFilesBackgroundWorker.ReportProgress(10);
            _clientTransferProtocol.RemoteConnectionInfo = _invoiceTemplateModel.GetSelectedTemplateConnectionInfo();
            _getFilesBackgroundWorker.ReportProgress(20);

            if (_clientTransferProtocol.CheckConnection())
            {
                _getFilesBackgroundWorker.ReportProgress(50);
                _fileList = _clientTransferProtocol.GetFileList();
            }
            
            _getFilesBackgroundWorker.ReportProgress(75);
        }

        void GetFilesBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ConnectAndGetFiles();
        }

        void GetFilesBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            SetUiProgressState(100, false, true);
            
            if (_fileList == null || _fileList.Count == 0)
                MessageBox.Show(this, "No files found", "Remote File Download");
            else
            {
                _filesCheckedListBox.Items.AddRange(_fileList.ToArray());
            }
        }

        void SetUiProgressState(int progressPercent, bool useWaitCursor, bool uiEnabled)
        {
            progressBar1.Value = progressPercent;
            Application.UseWaitCursor = useWaitCursor;
            _mainPanel.Enabled = uiEnabled;
        }

        void GetFilesBackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        void RefreshListButton_Click(object sender, EventArgs e)
        {
            StartGetFileListBackgroundWorker();
        }

        private void _selectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            for (int item = 0; item < _filesCheckedListBox.Items.Count; item++)
            {
                _filesCheckedListBox.SetItemChecked(item, _selectAllCheckBox.Checked);
            }
        }
    }
}
