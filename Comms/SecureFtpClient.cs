using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Renci.SshNet;

namespace zInvoiceTransformer.Comms
{
    public class SecureFtpClient : IClientTransferProtocol
    {
        ConnectionInfo _connectionInfo;
        public RemoteInvoiceConnectionInfo RemoteConnectionInfo { get; set; }

        public bool CheckConnection()
        {
           if (RemoteConnectionInfo == null) 
               throw new Exception("RemoteInvoiceConnectionInfo cannot be null");
                
           RemoteConnectionInfo.Validate();
           
           bool result = false;
           //PrivateKeyFile keyFile = new PrivateKeyFile(@"path/to/OpenSsh-RSA-key.ppk");
           //var keyFiles = new[] { keyFile };

           var methods = new List<AuthenticationMethod>
           {
               new PasswordAuthenticationMethod(RemoteConnectionInfo.Username, RemoteConnectionInfo.Password)
               //methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));
           };

           _connectionInfo = new ConnectionInfo(RemoteConnectionInfo.HostUrl, RemoteConnectionInfo.Port, RemoteConnectionInfo.Username, methods.ToArray());

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


        /// <summary>
        /// List a remote directory in the console.
        /// </summary>
        public List<string> GetFileList()
        {
            string remoteDirectory = RemoteConnectionInfo.RemoteFolder;
            var filelist = new List<string>();

            using (var sftp = new SftpClient(_connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    var files = sftp.ListDirectory(remoteDirectory);
                    //files = files.Where(f => !Regex.IsMatch(f.Name, @"^\.+"));

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

        public void DownloadFiles(List<string> filesToDownload)
        {
            throw new NotImplementedException();
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
