using System;
using System.Collections.Generic;
using Renci.SshNet;

namespace ZinvoiceTransformer
{
    public interface ITransferProtocol
    {
        bool CheckConnection(string host, int port, string username, string password);
        void CheckConnection();
        void CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo);
    }

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

        public void CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            throw new NotImplementedException();
        }
    }

    public class Sftp : ITransferProtocol
    {
        ConnectionInfo _connectionInfo;
        public bool CheckConnection(string host, int port, string username, string password)
        {
            bool result;
            //PrivateKeyFile keyFile = new PrivateKeyFile(@"path/to/OpenSsh-RSA-key.ppk");
            //var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>
            {
                new PasswordAuthenticationMethod(username, password)
                //methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));
            };

            _connectionInfo = new ConnectionInfo(host, port, username, methods.ToArray());

            using (var client = new SftpClient(_connectionInfo))
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

        public void CheckConnection()
        {
            throw new NotImplementedException();
        }

        public void CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            CheckConnection(
                remoteInvoiceConnectionInfo.Url,
                remoteInvoiceConnectionInfo.Port,
                remoteInvoiceConnectionInfo.Username,
                remoteInvoiceConnectionInfo.Password);
        }

        /// <summary>
        /// List a remote directory in the console.
        /// </summary>
        private void ListFiles()
        {
            string remoteDirectory = "/some/example/directory";

            using (SftpClient sftp = new SftpClient(_connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    var files = sftp.ListDirectory(remoteDirectory);

                    foreach (var file in files)
                    {
                        Console.WriteLine(file.Name);
                    }

                    sftp.Disconnect();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception has been caught " + e.ToString());
                }
            }
        }
    }

    public class Api : ITransferProtocol
    {
        public bool CheckConnection(string host, int port, string username, string password)
        {
            throw new NotImplementedException();
        }

        public void CheckConnection()
        {
            throw new NotImplementedException();
        }

        public void CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
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
