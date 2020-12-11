using System;

namespace zInvoiceTransformer.Comms
{
    public static class RemoteConnectionFactory
    {
        public static IClientTransferProtocol Build(int protocolType)
        {
            switch (protocolType)
            {
                case 0:
                    return null;
                case 1:
                    return new SecureFtpClient();
                default:
                    throw new ArgumentException("Unknown protocol type");
            }
        }
    }
}
