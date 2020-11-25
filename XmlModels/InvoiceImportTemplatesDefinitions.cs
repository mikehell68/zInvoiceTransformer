using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesDefinitions
    {

        private InvoiceImportTemplatesDefinitionsFieldName[] fieldNamesField;

        private InvoiceImportTemplatesDefinitionsFileFormatType[] fileFormatTypesField;

        private InvoiceImportTemplatesDefinitionsRemoteTransferProtocolType[] remoteTransferProtocolTypesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("FieldName", IsNullable = false)]
        public InvoiceImportTemplatesDefinitionsFieldName[] FieldNames
        {
            get
            {
                return this.fieldNamesField;
            }
            set
            {
                this.fieldNamesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("FileFormatType", IsNullable = false)]
        public InvoiceImportTemplatesDefinitionsFileFormatType[] FileFormatTypes
        {
            get
            {
                return this.fileFormatTypesField;
            }
            set
            {
                this.fileFormatTypesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("RemoteTransferProtocolType", IsNullable = false)]
        public InvoiceImportTemplatesDefinitionsRemoteTransferProtocolType[] RemoteTransferProtocolTypes
        {
            get
            {
                return this.remoteTransferProtocolTypesField;
            }
            set
            {
                this.remoteTransferProtocolTypesField = value;
            }
        }
    }
}