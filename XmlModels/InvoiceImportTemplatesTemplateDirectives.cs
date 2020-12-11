using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateDirectives
    {

        private InvoiceImportTemplatesTemplateDirectivesDirective directiveField;

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDirectivesDirective Directive
        {
            get
            {
                return this.directiveField;
            }
            set
            {
                this.directiveField = value;
            }
        }
    }
}