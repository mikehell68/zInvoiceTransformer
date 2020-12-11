using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateDetailFieldsField
    {

        private InvoiceImportTemplatesTemplateDetailFieldsFieldDelimited delimitedField;

        private byte fieldNameIdField;

        private byte directiveIdField;

        private bool directiveIdFieldSpecified;

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDetailFieldsFieldDelimited Delimited
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte DirectiveId
        {
            get
            {
                return this.directiveIdField;
            }
            set
            {
                this.directiveIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DirectiveIdSpecified
        {
            get
            {
                return this.directiveIdFieldSpecified;
            }
            set
            {
                this.directiveIdFieldSpecified = value;
            }
        }
    }
}