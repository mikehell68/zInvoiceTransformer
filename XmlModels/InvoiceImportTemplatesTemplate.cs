using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplate
    {

        private InvoiceImportTemplatesTemplateRemoteInvoiceSettings remoteInvoiceSettingsField;

        private InvoiceImportTemplatesTemplateMasterRow masterRowField;

        private InvoiceImportTemplatesTemplateDetailFields detailFieldsField;

        private InvoiceImportTemplatesTemplateSummaryRow summaryRowField;

        private InvoiceImportTemplatesTemplateDirectives directivesField;

        private InvoiceImportTemplatesTemplateEachesConversion eachesConversionField;

        private InvoiceImportTemplatesTemplateTemplateTransform templateTransformField;

        private string[] invoiceNumbersToUpdateField;

        private byte idField;

        private string nameField;

        private byte activeField;

        private byte templateVersionField;

        private string descriptionField;

        private byte fileFormatTypeIdField;

        private string sourceFolderField;

        private string outputFolderField;

        private string delimiterField;

        private byte lbProcessingTypeField;

        private byte hasHeaderRecordField;

        private byte hasMasterRecordField;

        private byte hasSummaryRecordField;

        private string uoMCaseField;

        private string uoMEachField;

        private string uoMWeightField;

        /// <remarks/>
        public InvoiceImportTemplatesTemplateRemoteInvoiceSettings RemoteInvoiceSettings
        {
            get
            {
                return this.remoteInvoiceSettingsField;
            }
            set
            {
                this.remoteInvoiceSettingsField = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateMasterRow MasterRow
        {
            get
            {
                return this.masterRowField;
            }
            set
            {
                this.masterRowField = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDetailFields DetailFields
        {
            get
            {
                return this.detailFieldsField;
            }
            set
            {
                this.detailFieldsField = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateSummaryRow SummaryRow
        {
            get
            {
                return this.summaryRowField;
            }
            set
            {
                this.summaryRowField = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateDirectives Directives
        {
            get
            {
                return this.directivesField;
            }
            set
            {
                this.directivesField = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateEachesConversion EachesConversion
        {
            get
            {
                return this.eachesConversionField;
            }
            set
            {
                this.eachesConversionField = value;
            }
        }

        /// <remarks/>
        public InvoiceImportTemplatesTemplateTemplateTransform TemplateTransform
        {
            get
            {
                return this.templateTransformField;
            }
            set
            {
                this.templateTransformField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("InvoiceNumber", IsNullable = false)]
        public string[] InvoiceNumbersToUpdate
        {
            get
            {
                return this.invoiceNumbersToUpdateField;
            }
            set
            {
                this.invoiceNumbersToUpdateField = value;
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Active
        {
            get
            {
                return this.activeField;
            }
            set
            {
                this.activeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte TemplateVersion
        {
            get
            {
                return this.templateVersionField;
            }
            set
            {
                this.templateVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte FileFormatTypeId
        {
            get
            {
                return this.fileFormatTypeIdField;
            }
            set
            {
                this.fileFormatTypeIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SourceFolder
        {
            get
            {
                return this.sourceFolderField;
            }
            set
            {
                this.sourceFolderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OutputFolder
        {
            get
            {
                return this.outputFolderField;
            }
            set
            {
                this.outputFolderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Delimiter
        {
            get
            {
                return this.delimiterField;
            }
            set
            {
                this.delimiterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte LbProcessingType
        {
            get
            {
                return this.lbProcessingTypeField;
            }
            set
            {
                this.lbProcessingTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte HasHeaderRecord
        {
            get
            {
                return this.hasHeaderRecordField;
            }
            set
            {
                this.hasHeaderRecordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte HasMasterRecord
        {
            get
            {
                return this.hasMasterRecordField;
            }
            set
            {
                this.hasMasterRecordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte HasSummaryRecord
        {
            get
            {
                return this.hasSummaryRecordField;
            }
            set
            {
                this.hasSummaryRecordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UoMCase
        {
            get
            {
                return this.uoMCaseField;
            }
            set
            {
                this.uoMCaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UoMEach
        {
            get
            {
                return this.uoMEachField;
            }
            set
            {
                this.uoMEachField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UoMWeight
        {
            get
            {
                return this.uoMWeightField;
            }
            set
            {
                this.uoMWeightField = value;
            }
        }
    }
}