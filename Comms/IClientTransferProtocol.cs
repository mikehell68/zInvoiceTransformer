using System.Collections.Generic;

namespace zInvoiceTransformer.Comms
{
    public interface IClientTransferProtocol
    {
        RemoteInvoiceConnectionInfo RemoteConnectionInfo { get; set; }
        bool CheckConnection();
        List<string> GetFileList();
        void DownloadFiles(List<string> filesToDownload);
        void UploadFile(string fileToUpload);
    }
}
