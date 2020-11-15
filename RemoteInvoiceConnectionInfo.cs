using System;

namespace zInvoiceTransformer
{
    public class RemoteInvoiceConnectionInfo
    {
        public string HostUrl { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string InvoiceFilePrefix { get; set; }
        public string DestinationFolder { get; set; }
        public string RemoteFolder { get; set; }

        public void Validate()
        {
            if(string.IsNullOrEmpty(HostUrl.Trim()))
                throw new Exception("HostUrl cannot be null or empty");
            if (string.IsNullOrEmpty(Username.Trim()))
                throw new Exception("Username cannot be null or empty");
            if (string.IsNullOrEmpty(Password.Trim()))
                throw new Exception("Password cannot be null or empty");
            if (string.IsNullOrEmpty(DestinationFolder.Trim()))
                throw new Exception("DestinationFolder cannot be null or empty");
        }
    }
}
