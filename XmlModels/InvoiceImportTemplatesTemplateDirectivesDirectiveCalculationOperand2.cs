using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateDirectivesDirectiveCalculationOperand2
    {

        private byte sourceFieldPositionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte sourceFieldPosition
        {
            get
            {
                return this.sourceFieldPositionField;
            }
            set
            {
                this.sourceFieldPositionField = value;
            }
        }
    }
}