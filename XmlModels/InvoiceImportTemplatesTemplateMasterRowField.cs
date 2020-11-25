using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateMasterRowField
    {

        private InvoiceImportTemplatesTemplateMasterRowFieldDelimited delimitedField;

        private byte fieldNameIdField;

        /// <remarks/>
        public InvoiceImportTemplatesTemplateMasterRowFieldDelimited Delimited
        {
            get
            {
                return this.delimitedField;
            }
            set
            {
                this.delimitedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte FieldNameId
        {
            get
            {
                return this.fieldNameIdField;
            }
            set
            {
                this.fieldNameIdField = value;
            }
        }
    }
}