using System;

namespace ZinvoiceTransformer.Comms
{
    public static class RemoteConnectionFactory
    {
        public static ITransferProtocol Build(int protocolType)
        {
            switch (protocolType)
            {
                case 0:
                    return null;
                case 1:
                    return new Ftp();
                case 2:
                    return new Sftp();
                case 3:
                    return new Api();
                default:
                    throw new ArgumentException("Unknown protocol type");
            }
        }
    }
}
