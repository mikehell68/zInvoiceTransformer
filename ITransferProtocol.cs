using System;
using System.Collections.Generic;
using Renci.SshNet;

namespace ZinvoiceTransformer
{
    public interface ITransferProtocol
    {
        bool Connect(string host, int port, string username, string password);
        void Connect();
        void Connect(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo);
    }

    public class Ftp : ITransferProtocol
    {
        public bool Connect(string host, int port, string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Connect(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            throw new NotImplementedException();
        }
    }

    public class Sftp : ITransferProtocol
    {
        public bool Connect(string host, int port, string username, string password)
        {
            bool result;
            //PrivateKeyFile keyFile = new PrivateKeyFile(@"path/to/OpenSsh-RSA-key.ppk");
            //var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>
            {
                new PasswordAuthenticationMethod(username, password)
                //methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));
            };
            
            var con = new ConnectionInfo(host, port, username, methods.ToArray());

            using (var client = new SftpClient(con))
            {
                try
                {
                
                    client.Connect();
                    result = client.IsConnected;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    client.Disconnect();
                }
            }

            return result;
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Connect(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            Connect(
                remoteInvoiceConnectionInfo.Url,
                remoteInvoiceConnectionInfo.Port,
                remoteInvoiceConnectionInfo.Username,
                remoteInvoiceConnectionInfo.Password);
        }
    }

    public class Api : ITransferProtocol
    {
        public bool Connect(string host, int port, string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Connect(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            throw new NotImplementedException();
        }
    }

    public static class RemoteConnectionFactory
    {
        public static ITransferProtocol Build(int protocolType)
        {
            switch (protocolType)
            {
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
