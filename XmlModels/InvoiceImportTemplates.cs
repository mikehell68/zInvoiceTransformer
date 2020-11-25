using System;

namespace ZinvoiceTransformer.XmlModels
{
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public partial class InvoiceImportTemplates
    {
        private InvoiceImportTemplatesDefinitions _definitionsField;

        private InvoiceImportTemplatesImportSettings _importSettingsField;

        private InvoiceImportTemplatesTemplate[] _templatesField;

        /// <remarks/>
        public InvoiceImportTemplatesDefinitions Definitions
        {
            get => _definitionsField;
            set => _definitionsField = value;
        }

        /// <remarks/>
        public InvoiceImportTemplatesImportSettings ImportSettings
        {
            get => _importSettingsField;
            set => _importSettingsField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem("Template", IsNullable = false)]
        public InvoiceImportTemplatesTemplate[] Templates
        {
            get => _templatesField;
            set => _templatesField = value;
        }
    }
}
