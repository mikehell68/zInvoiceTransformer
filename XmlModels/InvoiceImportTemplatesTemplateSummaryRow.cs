using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateSummaryRow
    {

        private InvoiceImportTemplatesTemplateSummaryRowField[] fieldField;

        private sbyte recordTypePostionField;

        private string recordTypeIdentifierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Field")]
        public InvoiceImportTemplatesTemplateSummaryRowField[] Field
        {
            get
            {
                return this.fieldField;
            }
            set
            {
                this.fieldField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public sbyte RecordTypePostion
        {
            get
            {
                return this.recordTypePostionField;
            }
            set
            {
                this.recordTypePostionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RecordTypeIdentifier
        {
            get
            {
                return this.recordTypeIdentifierField;
            }
            set
            {
                this.recordTypeIdentifierField = value;
            }
        }
    }
}