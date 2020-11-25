namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateSummaryRowFieldDelimited
    {

        private byte positionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }
    }
}