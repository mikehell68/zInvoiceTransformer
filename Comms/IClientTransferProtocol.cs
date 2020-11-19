using System;
using System.Collections.Generic;
using Renci.SshNet.Sftp;

namespace zInvoiceTransformer.Comms
{
    public interface IClientTransferProtocol
    {
        RemoteInvoiceConnectionInfo RemoteConnectionInfo { get; set; }
        bool CheckConnection();
        List<SftpFile> GetFileList();
        void DownloadFiles(List<string> filesToDownload);
        void DownloadFiles(List<SftpFile> filesToDownload, Action<long> progressAction);
        void UploadFile(string fileToUpload);
    }
}
