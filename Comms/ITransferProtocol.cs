using System.Collections.Generic;

namespace ZinvoiceTransformer.Comms
{
    public interface ITransferProtocol
    {
        bool CheckConnection(string host, int port, string username, string password);
        void CheckConnection();
        bool CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo);
        List<string> ListFiles();
        void UploadFile(string fileToUpload);
    }
}
