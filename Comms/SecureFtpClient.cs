using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Sftp;

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
           
           bool result;
           
           var methods = new List<AuthenticationMethod>
           {
               new PasswordAuthenticationMethod(RemoteConnectionInfo.Username, RemoteConnectionInfo.Password)
           };

           _connectionInfo = new ConnectionInfo(RemoteConnectionInfo.HostUrl, RemoteConnectionInfo.Port, RemoteConnectionInfo.Username, methods.ToArray());

           using (var client = new SftpClient(_connectionInfo))
           {
               try
               { 
                   client.Connect();
                   result = client.IsConnected;
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
        public List<SftpFile> GetFileList()
        {
            var remoteDirectory = RemoteConnectionInfo.RemoteFolder;
            var filenameFilter = RemoteConnectionInfo.InvoiceFilePrefix ?? "";
            var fileList = new List<SftpFile>();

            using (var sftp = new SftpClient(_connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    fileList = sftp.ListDirectory(remoteDirectory).
                        Where(f => !Regex.IsMatch(f.Name, @"^\.+") &&
                                   !f.IsDirectory &&
                                   !f.IsSymbolicLink &&
                                   f.IsRegularFile &&
                                   f.Name.Contains(filenameFilter)).ToList();
                   
                    sftp.Disconnect();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception has been caught " + e.ToString());
                }
            }
            return fileList;
        }

        public void DownloadFiles(List<string> filesToDownload)
        {
            string remoteFolder = RemoteConnectionInfo.RemoteFolder;
            string destinationFolder = RemoteConnectionInfo.DestinationFolder;
            
            try
            {
                using (var sftpClient = new SftpClient(_connectionInfo))
                {
                    sftpClient.Connect();

                    foreach (var file in filesToDownload)
                    {
                        using (var fs = new FileStream(Path.Combine(destinationFolder, file), FileMode.OpenOrCreate))
                        { 
                            sftpClient.DownloadFile(
                                Path.Combine(remoteFolder, file).Replace('\\', '/'),
                                fs,
                                downloaded =>
                                {
                                    Console.WriteLine(
                                        $"Downloaded {(double) downloaded / fs.Length * 100}% of the file.");
                                });
                        }
                    }
                    sftpClient.Disconnect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DownloadFiles(List<SftpFile> filesToDownload, Action<long> progressAction)
        {
            string remoteFolder = RemoteConnectionInfo.RemoteFolder;
            string destinationFolder = RemoteConnectionInfo.DestinationFolder;

            try
            {
                using (var sftpClient = new SftpClient(_connectionInfo))
                {
                    sftpClient.Connect();

                    foreach (var file in filesToDownload)
                    {
                        using (var fs = new FileStream(Path.Combine(destinationFolder, file.Name), FileMode.OpenOrCreate))
                        {
                            sftpClient.DownloadFile(
                                Path.Combine(remoteFolder, file.Name).Replace('\\', '/'),
                                fs,
                                downloaded => progressAction((long)downloaded));
                        }
                    }
                    sftpClient.Disconnect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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

        public void DeleteRemoteFiles(List<SftpFile> remoteFiles)
        {
            using (var sftpClient = new SftpClient(_connectionInfo))
            {
                try
                {
                    sftpClient.Connect();

                    foreach (var remoteFile in remoteFiles)
                    {
                        sftpClient.DeleteFile(remoteFile.FullName);
                    }
                    sftpClient.Disconnect();
                }
                catch (Exception er)
                {
                    Console.WriteLine("An exception has been caught " + er.ToString());
                }
            }
        }

        public void DeleteRemoteFile(SftpFile remoteFile)
        {
            using (var sftpClient = new SftpClient(_connectionInfo))
            {
                try
                {
                    sftpClient.Connect();
                    sftpClient.DeleteFile(remoteFile.FullName);
                    sftpClient.Disconnect();
                }
                catch (Exception er)
                {
                    Console.WriteLine("An exception has been caught " + er.ToString());
                }
            }
        }
    }
}
