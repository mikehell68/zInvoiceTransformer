using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateRemoteInvoiceSettings
    {

        private byte remoteTransferProtocolTypeIdField;

        private string urlField;

        private int portField;

        private string usernameField;

        private string passwordField;

        private string keyfileLocationField;

        private string remoteFolderField;

        private string invoiceFileCustomerPrefixField;

        private bool deleteRemoteFilesAfterDownloadField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte RemoteTransferProtocolTypeId
        {
            get
            {
                return this.remoteTransferProtocolTypeIdField;
            }
            set
            {
                this.remoteTransferProtocolTypeIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string username
        {
            get
            {
                return this.usernameField;
            }
            set
            {
                this.usernameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string keyfileLocation
        {
            get
            {
                return this.keyfileLocationField;
            }
            set
            {
                this.keyfileLocationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RemoteFolder
        {
            get
            {
                return this.remoteFolderField;
            }
            set
            {
                this.remoteFolderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string InvoiceFileCustomerPrefix
        {
            get
            {
                return this.invoiceFileCustomerPrefixField;
            }
            set
            {
                this.invoiceFileCustomerPrefixField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool DeleteRemoteFileAfterDownload
        {
            get
            {
                return this.deleteRemoteFilesAfterDownloadField;
            }
            set
            {
                this.deleteRemoteFilesAfterDownloadField = value;
            }
        }
    }
}