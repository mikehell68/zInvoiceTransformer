using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LogThis;
using Renci.SshNet.Sftp;
using zInvoiceTransformer.Comms;

namespace zInvoiceTransformer
{
    public partial class RemoteDownloadDialog : Form
    {
        readonly IClientTransferProtocol _clientTransferProtocol;
        List<SftpFile> _fileList;
        long _totalBytesToDownload = 0;

        public RemoteDownloadDialog(InvoiceTemplateModel invoiceTemplateModel,
            IClientTransferProtocol clientTransferProtocol)
        {
            InitializeComponent();
            _clientTransferProtocol = clientTransferProtocol;
            //_getFilesBackgroundWorker.WorkerReportsProgress = true;
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
            SetUiProgressState(true, false, null, 0);

            _getFilesBackgroundWorker.RunWorkerAsync();
        }

        void StartFileDownloadBackgroundWorker()
        {
            if (_downloadFilesBackgroundWorker.IsBusy)
                return;

            SetUiProgressState(true, false, null, 0);

            _downloadFilesBackgroundWorker.RunWorkerAsync();
        }

        void ClearFileList()
        {
            _selectAllCheckBox.Checked = false;
            _filesCheckedListBox.Items.Clear();
            _fileList?.Clear();
        }

        void ConnectAndGetFileList()
        {
            try
            {
                _getFilesBackgroundWorker.ReportProgress(20, "Checking Connection");

                if (_clientTransferProtocol.CheckConnection())
                {
                    _getFilesBackgroundWorker.ReportProgress(50, "Fetching remote file names");
                    _fileList = _clientTransferProtocol.GetFileList();
                }

                _getFilesBackgroundWorker.ReportProgress(75, "Fetching remote file names - complete");
                _totalBytesToDownload = _fileList.Sum(f => f.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogThis(ex.Message + '\n' + ex.StackTrace, eloglevel.error);
            }
        }

        void ConnectAndDownloadSelectedFiles(List<SftpFile> selectedFiles)
        {
            try
            {
                _downloadFilesBackgroundWorker.ReportProgress(0, "Checking Connection");
                if (_clientTransferProtocol.CheckConnection())
                {
                    _clientTransferProtocol.DownloadFiles(selectedFiles, progress => UpdateProgressBar(progress));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogThis(ex.Message + '\n' + ex.StackTrace, eloglevel.error);
            }
        }

        void GetFilesBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ConnectAndGetFileList();
        }

        void GetFilesBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            SetUiProgressState(false, true, null, 100);
            
            if (_fileList == null || _fileList.Count == 0)
                MessageBox.Show(this, "No files found", "Remote File Download");
            else
            {
                _filesCheckedListBox.Items.AddRange(_fileList.Select(f => f.Name).ToArray());
            }
        }

        void SetUiProgressState(bool useWaitCursor, bool uiEnabled, string progressText = null, int progressPercent = -1)
        {
            if(progressText != null)
                _progressLabel.Text = progressText;

            if(progressPercent >= 0)
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
            _progressBar.Maximum = 100;
            StartFileDownloadBackgroundWorker();
        }

        void UpdateProgressBar(decimal progress)
        {
            if(progress > 0) 
                _downloadFilesBackgroundWorker.ReportProgress((int)(progress / _totalBytesToDownload * 100), "Downloading...");
        }

        private void _downloadFilesBackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            _progressBar.Value = e.ProgressPercentage;
            _progressLabel.Text = e.UserState.ToString();
        }

        private void _downloadFilesBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var x = _filesCheckedListBox.CheckedItems.Cast<string>().ToList();
            ConnectAndDownloadSelectedFiles(_fileList.Where(f => x.Contains(f.Name)).ToList());
        }

        private void _downloadFilesBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            SetUiProgressState(false, true);
            _progressLabel.Text += " complete";
            //_downloadFilesBackgroundWorker.ReportProgress(100, _progressLabel.Text+" - complete");
        }
    }
}
