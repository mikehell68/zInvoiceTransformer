using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LogThis;
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
        /// List a remote directory.
        /// </summary>
        public List<SftpFile> GetFileList()
        {
            Log.LogThis("GetFileList()", eloglevel.info);
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
                    Log.LogThis($"An exception has been caught {e}", eloglevel.error);
                }
            }
            return fileList;
        }

        public void DownloadFiles(List<SftpFile> filesToDownload, Action<long> progressAction)
        {
            Log.LogThis("DownloadFiles(List<SftpFile> filesToDownload, Action)", eloglevel.info);
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
                Log.LogThis($"Exception while downloading files: {e.Message}", eloglevel.error);
            }
        }

        public void UploadFile(string fileToUpload)
        {
            Log.LogThis($"UploadFile({fileToUpload})", eloglevel.info);
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
                            Log.LogThis($"Uploaded {(double)uploaded / fs.Length * 100}% of the file.", eloglevel.info);
                        });

                    sftpClient.Disconnect();
                }
            }
            catch (Exception e)
            {
                Log.LogThis($"Exception uploading file: {e.Message}", eloglevel.error);
            }
        }

        public void DeleteRemoteFiles(List<SftpFile> remoteFiles)
        {
            Log.LogThis($"DeleteRemoteFiles()", eloglevel.info);
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
                    Log.LogThis($"Exception while deleting remote files: {er}", eloglevel.error);
                }
            }
        }

        public void DeleteRemoteFile(SftpFile remoteFile)
        {
            Log.LogThis($"DeleteRemoteFile()", eloglevel.info);
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
                    Log.LogThis($"Exception while deleting remote file: {er}", eloglevel.error);
                }
            }
        }
    }
}
