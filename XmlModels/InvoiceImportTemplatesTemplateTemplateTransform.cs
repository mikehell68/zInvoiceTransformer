using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateTemplateTransform
    {

        private InvoiceImportTemplatesTemplateTemplateTransformField[] fieldsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Field", IsNullable = false)]
        public InvoiceImportTemplatesTemplateTemplateTransformField[] Fields
        {
            get
            {
                return this.fieldsField;
            }
            set
            {
                this.fieldsField = value;
            }
        }
    }
}