using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateDirectivesDirectiveCalculation
    {

        private InvoiceImportTemplatesTemplateDirectivesDirectiveCalculationOperand1 operand1Field;

        private InvoiceImportTemplatesTemplateDirectivesDirectiveCalculationOperand2 operand2Field;

        private InvoiceImportTemplatesTemplateDirectivesDirectiveCalculationOperator operatorField;

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDirectivesDirectiveCalculationOperand1 Operand1
        {
            get
            {
                return this.operand1Field;
            }
            set
            {
                this.operand1Field = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDirectivesDirectiveCalculationOperand2 Operand2
        {
            get
            {
                return this.operand2Field;
            }
            set
            {
                this.operand2Field = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDirectivesDirectiveCalculationOperator Operator
        {
            get
            {
                return this.operatorField;
            }
            set
            {
                this.operatorField = value;
            }
        }
    }
}