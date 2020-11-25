using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesImportSettings
    {

        private InvoiceImportTemplatesImportSettingsImportAppliction importApplictionField;

        private InvoiceImportTemplatesImportSettingsTemplate[] importOrderField;

        /// <remarks/>
        public InvoiceImportTemplatesImportSettingsImportAppliction ImportAppliction
        {
            get
            {
                return this.importApplictionField;
            }
            set
            {
                this.importApplictionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Template", IsNullable = false)]
        public InvoiceImportTemplatesImportSettingsTemplate[] ImportOrder
        {
            get
            {
                return this.importOrderField;
            }
            set
            {
                this.importOrderField = value;
            }
        }
    }
}