using System;
using System.Collections.Generic;

namespace ZinvoiceTransformer.Comms
{
    public class Ftp : ITransferProtocol
    {
        public bool CheckConnection(string host, int port, string username, string password)
        {
            throw new NotImplementedException();
        }

        public void CheckConnection()
        {
            throw new NotImplementedException();
        }

        public bool CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            throw new NotImplementedException();
        }

        public List<string> ListFiles()
        {
            throw new NotImplementedException();
        }

        public void UploadFile(string fileToUpload)
        {
            throw new NotImplementedException();
        }
    }
}
