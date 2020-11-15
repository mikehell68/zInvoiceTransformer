using System;
using System.Collections.Generic;

namespace zInvoiceTransformer.Comms
{
    public class ApiClient : IClientTransferProtocol
    {
        public RemoteInvoiceConnectionInfo RemoteConnectionInfo { get; set; }

        public bool CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            throw new NotImplementedException();
        }

        public bool CheckConnection()
        {
            throw new NotImplementedException();
        }

        public List<string> GetFileList()
        {
            throw new NotImplementedException();
        }

        public void DownloadFiles(List<string> filesToDownload)
        {
            throw new NotImplementedException();
        }

        public void UploadFile(string fileToUpload)
        {
            throw new NotImplementedException();
        }
    }
}
