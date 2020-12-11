using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateMasterRowFieldDelimited
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