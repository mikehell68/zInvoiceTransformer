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
        //public RemoteDownloadDialog()
        //{
        //    InitializeComponent();
        //}

        public RemoteDownloadDialog(InvoiceTemplateModel invoiceTemplateModel)
        {
            InitializeComponent();
            _invoiceTemplateModel = invoiceTemplateModel;
            _getFilesBackgroundWorker.WorkerReportsProgress = true;
            InitialiseClientConnectionDetails();
        }

        private void InitialiseClientConnectionDetails()
        {
            if(_invoiceTemplateModel != null)
                _clientTransferProtocol = RemoteConnectionFactory.Build(
                Convert.ToInt32(
                    _invoiceTemplateModel.SelectedTemplate
                        ?.Element("RemoteInvoiceSettings")
                        ?.Attribute("RemoteTransferProtocolTypeId")
                        ?.Value ?? "0"));
        }

        private void RemoteDownloadDialog_Shown(object sender, EventArgs e)
        {
            StartGetFileListBackgroundWorker();
        }

        void StartGetFileListBackgroundWorker()
        {
            if(_getFilesBackgroundWorker.IsBusy)
                return;
            
            _filesCheckedListBox.Items.Clear();
            _fileList?.Clear();
            progressBar1.Value = 0;
            _getFilesBackgroundWorker.RunWorkerAsync();
            //while (_getFilesBackgroundWorker.IsBusy)
            //{
            //    progressBar1.Increment(1);
            //    Application.DoEvents();
            //}
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

        private void _getFilesBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            
            try
            {
                ConnectAndGetFiles();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void _getFilesBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (_fileList == null || _fileList.Count == 0)
                MessageBox.Show(this, "No files found", "Remote Files");
            else
            {
                _filesCheckedListBox.Items.AddRange(_fileList.ToArray());
            }
            progressBar1.Value = 100;
        }

        private void _getFilesBackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void _refreshListButton_Click(object sender, EventArgs e)
        {
            StartGetFileListBackgroundWorker();
        }
    }
}
