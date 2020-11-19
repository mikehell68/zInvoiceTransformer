using System;
using System.Collections.Generic;
using Renci.SshNet.Sftp;

namespace zInvoiceTransformer.Comms
{
    public class FtpClient : IClientTransferProtocol
    {
        public RemoteInvoiceConnectionInfo RemoteConnectionInfo { get; set; }

        public bool CheckConnection()
        {
            throw new NotImplementedException();
        }

        public bool CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            throw new NotImplementedException();
        }

        public List<SftpFile> GetFileList()
        {
            throw new NotImplementedException();
        }

        public void DownloadFiles(List<string> filesToDownload)
        {
            throw new NotImplementedException();
        }

        public void DownloadFiles(List<SftpFile> filesToDownload, Action<long> progressAction)
        {
            throw new NotImplementedException();
        }

        public void UploadFile(string fileToUpload)
        {
            throw new NotImplementedException();
        }
    }
}
