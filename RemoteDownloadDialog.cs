using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using zInvoiceTransformer.Comms;

namespace zInvoiceTransformer
{
    public partial class RemoteDownloadDialog : Form
    {
        IClientTransferProtocol _clientTransferProtocol;
        readonly InvoiceTemplateModel _invoiceTemplateModel;
        private List<string> _fileList;

        public RemoteDownloadDialog(InvoiceTemplateModel invoiceTemplateModel,
            IClientTransferProtocol clientTransferProtocol)
        {
            InitializeComponent();
            _invoiceTemplateModel = invoiceTemplateModel;
            _clientTransferProtocol = clientTransferProtocol;
            _getFilesBackgroundWorker.WorkerReportsProgress = true;
            PopulateRemoteConnectionLabels();
        }

        private void PopulateRemoteConnectionLabels()
        {
            _hostLabel.Text = _clientTransferProtocol.RemoteConnectionInfo.HostUrl;
            _destinationLabel.Text = _clientTransferProtocol.RemoteConnectionInfo.DestinationFolder;
            _portLabel.Text = _clientTransferProtocol.RemoteConnectionInfo.Port.ToString();
            _remoteLabel.Text = _clientTransferProtocol.RemoteConnectionInfo.RemoteFolder;
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

        void ConnectAndGetFileList()
        {
            _getFilesBackgroundWorker.ReportProgress(20, "Checking Connection");

            if (_clientTransferProtocol.CheckConnection())
            {
                _getFilesBackgroundWorker.ReportProgress(50, "Fetching remote file names");
                _fileList = _clientTransferProtocol.GetFileList();
            }
            
            _getFilesBackgroundWorker.ReportProgress(75, "Fetching remote file names - complete");
        }

        void ConnectAndDownloadSelectedFiles(List<string> selectedFiles)
        {
            if (_clientTransferProtocol.CheckConnection())
            {
                _clientTransferProtocol.DownloadFiles(selectedFiles);
            }
        }

        void GetFilesBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ConnectAndGetFileList();
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
            _progressLabel.Text = "";
            _progressBar.Value = progressPercent;
            Application.UseWaitCursor = useWaitCursor;
            _mainPanel.Enabled = uiEnabled;
        }

        void GetFilesBackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            _progressBar.Value = e.ProgressPercentage;
            _progressLabel.Text = e.UserState.ToString();
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

        private void _downloadFilesButton_Click(object sender, EventArgs e)
        {
            var x = _filesCheckedListBox.CheckedItems.Cast<string>().ToList();
            ConnectAndDownloadSelectedFiles(x);
        }
    }
}
