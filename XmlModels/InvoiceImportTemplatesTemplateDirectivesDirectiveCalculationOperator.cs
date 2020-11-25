using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateDirectivesDirectiveCalculationOperator
    {

        private string opTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OpType
        {
            get
            {
                return this.opTypeField;
            }
            set
            {
                this.opTypeField = value;
            }
        }
    }
}