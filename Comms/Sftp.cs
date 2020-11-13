using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Renci.SshNet;

namespace ZinvoiceTransformer.Comms
{
    public class Sftp : ITransferProtocol
    {
        ConnectionInfo _connectionInfo;
        public bool CheckConnection(string host, int port, string username, string password)
        {
            bool result = false;
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
                    //throw;
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

        public bool CheckConnection(RemoteInvoiceConnectionInfo remoteInvoiceConnectionInfo)
        {
            return CheckConnection(
                remoteInvoiceConnectionInfo.Url,
                remoteInvoiceConnectionInfo.Port,
                remoteInvoiceConnectionInfo.Username,
                remoteInvoiceConnectionInfo.Password);
        }

        /// <summary>
        /// List a remote directory in the console.
        /// </summary>
        public List<string> ListFiles()
        {
            string remoteDirectory = "./inv";
            var filelist = new List<string>();

            using (SftpClient sftp = new SftpClient(_connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    var files = sftp.ListDirectory(remoteDirectory);
                    files = files.Where(f => !Regex.IsMatch(f.Name, @"^\.+"));

                    foreach (var file in files)
                    {
                        filelist.Add(file.Name);
                        Console.WriteLine(file.Name);
                    }

                    sftp.Disconnect();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception has been caught " + e.ToString());
                }
            }
            return filelist;
        }

        public void UploadFile(string fileToUpload)
        {
            try
            {
                using (var sftpClient = new SftpClient(_connectionInfo))
                using (var fs = new FileStream(fileToUpload, FileMode.Open))
                {
                    sftpClient.Connect();

                    sftpClient.UploadFile(
                        fs,
                        "/upload/" + Path.GetFileName(fileToUpload),
                        uploaded =>
                        {
                            Console.WriteLine($"Uploaded {(double)uploaded / fs.Length * 100}% of the file.");
                        });

                    sftpClient.Disconnect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
