using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesImportSettingsImportAppliction
    {

        private string fileNameField;

        private string invoiceFileLocationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string InvoiceFileLocation
        {
            get
            {
                return this.invoiceFileLocationField;
            }
            set
            {
                this.invoiceFileLocationField = value;
            }
        }
    }
}