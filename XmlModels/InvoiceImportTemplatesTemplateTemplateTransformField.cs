using System;

namespace ZinvoiceTransformer.XmlModels
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class InvoiceImportTemplatesTemplateTemplateTransformField
    {

        private byte fieldNameIdField;

        private string dataTypeField;

        private string nameField;

        private byte startField;

        private byte lengthField;

        private string dateFormatField;

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
        public string DataType
        {
            get
            {
                return this.dataTypeField;
            }
            set
            {
                this.dataTypeField = value;
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
        public byte Start
        {
            get
            {
                return this.startField;
            }
            set
            {
                this.startField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Length
        {
            get
            {
                return this.lengthField;
            }
            set
            {
                this.lengthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DateFormat
        {
            get
            {
                return this.dateFormatField;
            }
            set
            {
                this.dateFormatField = value;
            }
        }
    }
}