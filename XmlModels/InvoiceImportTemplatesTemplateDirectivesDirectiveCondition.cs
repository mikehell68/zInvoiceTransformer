using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateDirectivesDirectiveCondition
    {

        private byte conditionFieldPositionField;

        private byte conditionValueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte ConditionFieldPosition
        {
            get
            {
                return this.conditionFieldPositionField;
            }
            set
            {
                this.conditionFieldPositionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte ConditionValue
        {
            get
            {
                return this.conditionValueField;
            }
            set
            {
                this.conditionValueField = value;
            }
        }
    }
}