namespace ZinvoiceTransformer
{
    public class RemoteInvoiceConnectionInfo
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string InvoiceFilePrefix { get; set; }
        public string DestinationFolder { get; set; }
    }
}
