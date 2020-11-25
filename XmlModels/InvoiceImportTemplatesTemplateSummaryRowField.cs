namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateSummaryRowField
    {

        private InvoiceImportTemplatesTemplateSummaryRowFieldDelimited delimitedField;

        private byte fieldNameIdField;

        /// <remarks/>
        public InvoiceImportTemplatesTemplateSummaryRowFieldDelimited Delimited
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