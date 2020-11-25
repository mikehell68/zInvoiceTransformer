using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateDirectivesDirective
    {

        private InvoiceImportTemplatesTemplateDirectivesDirectiveCondition conditionField;

        private InvoiceImportTemplatesTemplateDirectivesDirectiveCalculation calculationField;

        private byte idField;

        private string nameField;

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDirectivesDirectiveCondition Condition
        {
            get
            {
                return this.conditionField;
            }
            set
            {
                this.conditionField = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDirectivesDirectiveCalculation Calculation
        {
            get
            {
                return this.calculationField;
            }
            set
            {
                this.calculationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }
}