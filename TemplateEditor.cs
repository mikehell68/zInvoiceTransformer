using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System;
using ZinvoiceTransformer.Comms;

namespace ZinvoiceTransformer
{
    public partial class TemplateEditor : Form
    {
        static XDocument _invoiceImportTemplates;
        XElement _selectedTemplateForDisplay;
        XElement _originalTemplate;
        InvoiceTemplateModel _invoiceTemplateModel;
        bool _isInitialLoad = true;
        string _fieldTemplate =
@"<Field FieldNameId="""" > 
    <Delimited Position="""" />
  </Field>";
        

        public TemplateEditor(InvoiceTemplateModel invoiceTemplateModel)
        {
            InitializeComponent();
            _invoiceTemplateModel = invoiceTemplateModel;
            _templatesListBox.SelectedIndexChanged += _templatesListBoxSelectionChanged;
            LoadTemplates();
            _isInitialLoad = false;
        }

        private void LoadTemplates(string selectedTemplateId = null)
        {
            _invoiceImportTemplates = _invoiceTemplateModel.InvoiceImportTemplates;

            var allTemplateListItems = _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Select(x => new TemplateListItem { Id = x.Attribute("Id").Value, Name = x.Attribute("Name").Value, IsInUse = x.Attribute("Active").Value == "1" }).ToArray();
            _templatesListBox.Items.Clear();
            _templatesListBox.Items.AddRange(allTemplateListItems);

            if (string.IsNullOrEmpty(selectedTemplateId) && _invoiceTemplateModel.SelectedTemplate != null)
                SelectedTemplateId = _invoiceTemplateModel.SelectedTemplate.Attribute("Id").Value;
            else
                SelectedTemplateId = selectedTemplateId;
        }

        void _templatesListBoxSelectionChanged(object sender, System.EventArgs e)
        {
            if(!_isInitialLoad)
                SaveSelectedTemplateEdits();
            
            if (!_isInitialLoad && SelectedTemplateHasEdits())
            {
                var result = MessageBox.Show(this,
                                string.Format("The selected template '{0}' has changed.\nSave changes?", _selectedTemplateForDisplay.Attribute("Name").Value),
                                Text,
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        _invoiceTemplateModel.SelectedTemplate = _invoiceTemplateModel.GetTemplate(((TemplateListItem)_templatesListBox.SelectedItem).Id);
                        SaveTemplates();
                        break;
                    case DialogResult.No:
                        _invoiceTemplateModel.SelectedTemplate = _invoiceTemplateModel.GetTemplate(((TemplateListItem)_templatesListBox.SelectedItem).Id);
                        break;
                    case DialogResult.Cancel:
                    default:
                        _templatesListBox.SelectedIndexChanged -= _templatesListBoxSelectionChanged;
                        _templatesListBox.SelectedItem = _templatesListBox.Items.Cast<TemplateListItem>().First(t => t.Id.ToString() == _selectedTemplateForDisplay.Attribute("Id").Value);
                        _templatesListBox.SelectedIndexChanged += _templatesListBoxSelectionChanged;
                        return;
                }
            }
            else
            {
                _invoiceTemplateModel.SelectedTemplate = _invoiceTemplateModel.GetTemplate(((TemplateListItem)_templatesListBox.SelectedItem).Id);
            }

            LoadTemplate();
        }

        private void LoadTemplate()
        {
            _templateFieldDefinitionsFlowLayoutPanel.SuspendLayout();

            _templateFieldDefinitionsFlowLayoutPanel.Controls.Clear();

            if (_templatesListBox.SelectedItem != null)
            {
                var item = (TemplateListItem)_templatesListBox.SelectedItem;

                _originalTemplate = _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").FirstOrDefault(x => x.Attribute("Id").Value == item.Id);
                CreateRemoteInvoiceSettingsElementIfRequired();
                _selectedTemplateForDisplay = new XElement(_originalTemplate);

                _nameTextBox.Text = _selectedTemplateForDisplay.Attribute("Name").Value;
                _descriptionTextBox.Text = _selectedTemplateForDisplay.Attribute("Description").Value;
                _activeCheckBox.Checked = _selectedTemplateForDisplay.Attribute("Active").Value == "1";
                _sourceFolderTextBox.Text = _selectedTemplateForDisplay.Attribute("SourceFolder").Value;
                _outputFolderTextBox.Text = _selectedTemplateForDisplay.Attribute("OutputFolder").Value;
                _fieldDelimiterTextBox.Text = _selectedTemplateForDisplay.Attribute("Delimiter").Value;
                _lbProcessingTypeComboBox.SelectedIndex = SetWeightProcessingRule(_selectedTemplateForDisplay.Attribute("LbProcessingType").Value);
                _hasHeaderCheckBox.Checked = _selectedTemplateForDisplay.Attribute("HasHeaderRecord").Value == "1";
                _hasMasterCheckBox.Checked = _selectedTemplateForDisplay.Attribute("HasMasterRecord").Value == "1";
                _hasFooterCheckBox.Checked = _selectedTemplateForDisplay.Attribute("HasSummaryRecord").Value == "1";
                _uomCaseSymbolTextBox.Text = _selectedTemplateForDisplay.Attribute("UoMCase").Value;
                _uomEachSymbolTextBox.Text = _selectedTemplateForDisplay.Attribute("UoMEach").Value;
                _uomWeightSymbolTextBox.Text = _selectedTemplateForDisplay.Attribute("UoMWeight").Value;

                if (_hasMasterCheckBox.Checked)
                {
                    _masterRecordIdentifierTextBox.Text = _selectedTemplateForDisplay.Element("MasterRow").Attribute("RecordTypeIdentifier").Value;
                    _masterRecordPositionNumericUpDown.Value = int.Parse(_selectedTemplateForDisplay.Element("MasterRow").Attribute("RecordTypePostion").Value);
                }
                else
                {
                    _masterRecordIdentifierTextBox.Text = "";
                    _masterRecordPositionNumericUpDown.Value = -1;
                }

                _detailRecordIdentifierTextBox.Text = _selectedTemplateForDisplay.Element("DetailFields").Attribute("RecordTypeIdentifier").Value;
                _detailRecordPositionNumericUpDown.Value = int.Parse(_selectedTemplateForDisplay.Element("DetailFields").Attribute("RecordTypePostion").Value);

                if (_hasFooterCheckBox.Checked)
                {
                    _footerRecordIdentifierTextBox.Text = _selectedTemplateForDisplay.Element("SummaryRow").Attribute("RecordTypeIdentifier").Value;
                    _footerRecordPositionNumericUpDown.Value = int.Parse(_selectedTemplateForDisplay.Element("SummaryRow").Attribute("RecordTypePostion").Value);
                }
                else
                {
                    _footerRecordIdentifierTextBox.Text = "";
                    _footerRecordPositionNumericUpDown.Value = -1;
                }

                var fieldNameDefinitions = _invoiceImportTemplates.Root.Element("Definitions").Element("FieldNames");
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.MasterRow));
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.DetailFields));
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.SummaryRow));

                //var noOfmasterFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.MasterRow);
                //var noOfDetailFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.DetailFields);
                //var noOfFooterFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.SummaryRow);

                var eachesConversionElement = _selectedTemplateForDisplay.Element("EachesConversion");
                _useEachesConversionCheckbox.Checked = eachesConversionElement != null && eachesConversionElement.Attribute("enabled").Value == "1";
                _eachesConversionTagTextBox.Text = eachesConversionElement != null ? eachesConversionElement.Attribute("tag").Value : string.Empty;
                _eachesConversionTagTextBox.Enabled = _useEachesConversionCheckbox.Checked;
                
                var transferProtocols = _invoiceImportTemplates.Root.Element("Definitions")
                    .Element("RemoteTransferProtocolTypes")
                    .Descendants("RemoteTransferProtocolType")
                    .Select(el => new { id = Convert.ToInt32(el.Attribute("Id").Value), name = el.Attribute("Name").Value }).ToArray();

                _protocolTypeComboBox.DataSource = transferProtocols;
                _protocolTypeComboBox.DisplayMember = "name";
                _protocolTypeComboBox.ValueMember = "id";
                _protocolTypeComboBox.SelectedValue = Convert.ToInt32(_selectedTemplateForDisplay.Element("RemoteInvoiceSettings")?.Attribute("RemoteTransferProtocolTypeId").Value);

                _urlTextbox.Text = _selectedTemplateForDisplay.Element("RemoteInvoiceSettings")?.Attribute("url").Value;
                _portTextbox.Text = _selectedTemplateForDisplay.Element("RemoteInvoiceSettings")?.Attribute("port").Value;
                _usernameTextbox.Text = _selectedTemplateForDisplay.Element("RemoteInvoiceSettings")?.Attribute("username").Value;
                _passwordTextbox.Text = _selectedTemplateForDisplay.Element("RemoteInvoiceSettings")?.Attribute("password").Value;
                _keyfileLocationTextbox.Text = _selectedTemplateForDisplay.Element("RemoteInvoiceSettings")?.Attribute("keyfileLocation").Value;
                _invoiceFilePrefixTextBox.Text = _selectedTemplateForDisplay.Element("RemoteInvoiceSettings")?.Attribute("InvoiceFileCustomerPrefix").Value;
                _remoteFolderTextbox.Text = _selectedTemplateForDisplay.Element("RemoteInvoiceSettings")?.Attribute("RemoteFolder").Value;
            }

            _templateFieldDefinitionsFlowLayoutPanel.ResumeLayout();
        }

        private void CreateRemoteInvoiceSettingsElementIfRequired()
        {
            if (_originalTemplate.Element("RemoteInvoiceSettings") == null)
                _originalTemplate.Add(new XElement("RemoteInvoiceSettings",
                    new XAttribute("RemoteTransferProtocolTypeId", 0),
                    new XAttribute("url", ""),
                    new XAttribute("port", 0),
                    new XAttribute("username", ""),
                    new XAttribute("password", ""),
                    new XAttribute("keyfileLocation", ""),
                    new XAttribute("RemoteFolder", ""),
                    new XAttribute("InvoiceFileCustomerPrefix", "")));
        }

        private int SetWeightProcessingRule(string weightProcessingRule)
        {
            switch (weightProcessingRule)
            {
                case "1":
                    return 0;

                case "2":
                    return 1;

                case "3":
                default:
                    return 2;
            }
        }

        private string GetWeightProcessingRule(int weightProcessingRule)
        {
            switch (weightProcessingRule)
            {
                case 0:
                    return "1";

                case 1:
                    return "2";

                case 2:
                default:
                    return "3";
            }
        }

        private TemplateFieldDefinition[] CreateFieldDisplayItems(XElement fieldNameDefinitions, FieldRecordLocation fieldRecordLocation)
        {
            var templateFieldDefinitions = new List<TemplateFieldDefinition>();

            foreach (var fieldElement in _selectedTemplateForDisplay.Element(fieldRecordLocation.ToString()).Descendants("Field"))
            {
                XElement field = fieldElement;
                var fieldName = fieldNameDefinitions.Descendants("FieldName").Where(f => f.Attribute("Id").Value == field.Attribute("FieldNameId").Value).FirstOrDefault();

                int? directiveId = null;
                if(field.Attribute("DirectiveId") != null)
                    directiveId = int.Parse(field.Attribute("DirectiveId").Value);
                                
                templateFieldDefinitions.Add(new TemplateFieldDefinition(fieldName.Attribute("Name").Value,
                                                                         int.Parse(fieldName.Attribute("Id").Value),
                                                                         int.Parse(field.Element("Delimited").Attribute("Position").Value), 
                                                                         (int)fieldRecordLocation,
                                                                         directiveId));
            }
            return templateFieldDefinitions.ToArray();
        }

        private void SaveTemplates()
        {
            _originalTemplate.ReplaceWith(_selectedTemplateForDisplay);
            _invoiceImportTemplates.Save(InvoiceTemplateModel.InvoiceImportTemplatePath);
            _isInitialLoad = true;
            LoadTemplates();
            _isInitialLoad = false;
        }

        private void _closeButton_Click(object sender, System.EventArgs e)
        {
            SaveSelectedTemplateEdits();
            Close();
        }

        private void _deleteTemplateButton_Click(object sender, System.EventArgs e)
        {
            if (_templatesListBox.SelectedItem != null)
            {
                var item = (TemplateListItem)_templatesListBox.SelectedItem;

                if(MessageBox.Show(this, string.Format("Delete selected template '{0}'", item.Name), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Where(x => x.Attribute("Id").Value == item.Id).FirstOrDefault().Remove();
                    SaveTemplates();
                    LoadTemplates();
                }
            }
            else
            {
                MessageBox.Show(this, "No template selected", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void _selectKeyfileLocationButton_Click(object sender, System.EventArgs e)
        {
            _folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (_folderDialog.ShowDialog(this) == DialogResult.OK)
                _keyfileLocationTextbox.Text = _folderDialog.SelectedPath;
        }

        private void _testRemoteTransferProtocolButton_Click(object sender, System.EventArgs e)
        {
            // TODO: Hook up server connection
            var rc = RemoteConnectionFactory.Build((int)_protocolTypeComboBox.SelectedValue);
            var connected = rc.CheckConnection(_urlTextbox.Text, Convert.ToInt32(_portTextbox.Text), _usernameTextbox.Text, _passwordTextbox.Text);

            MessageBox.Show(this,$"Connection to {_urlTextbox.Text}: " + (connected ? "ok" : "failed"), "Test Connection", MessageBoxButtons.OK);
        }

        private void _saveButton_Click(object sender, System.EventArgs e)
        {
            if (_selectedTemplateForDisplay == null)
                return;

            SaveSelectedTemplateEdits();
            if (SelectedTemplateHasEdits())
            {
                SaveTemplates();
            }
        }

        private void SaveSelectedTemplateEdits()
        {
            if (_selectedTemplateForDisplay == null)
                return;

            _selectedTemplateForDisplay.Attribute("Name").Value = _nameTextBox.Text;
            _selectedTemplateForDisplay.Attribute("Description").Value = _descriptionTextBox.Text;
            _selectedTemplateForDisplay.Attribute("Active").Value = _activeCheckBox.Checked ? "1" : "0";
            _selectedTemplateForDisplay.Attribute("SourceFolder").Value = _sourceFolderTextBox.Text;
            _selectedTemplateForDisplay.Attribute("OutputFolder").Value = _outputFolderTextBox.Text;
            _selectedTemplateForDisplay.Attribute("Delimiter").Value = _fieldDelimiterTextBox.Text;
            _selectedTemplateForDisplay.Attribute("LbProcessingType").Value = GetWeightProcessingRule(_lbProcessingTypeComboBox.SelectedIndex);
            _selectedTemplateForDisplay.Attribute("HasHeaderRecord").Value = _hasHeaderCheckBox.Checked ? "1" : "0";

            _selectedTemplateForDisplay.Element("RemoteInvoiceSettings").SetAttributeValue("RemoteTransferProtocolTypeId", _protocolTypeComboBox.SelectedValue);
            _selectedTemplateForDisplay.Element("RemoteInvoiceSettings").SetAttributeValue("url", _urlTextbox.Text);
            _selectedTemplateForDisplay.Element("RemoteInvoiceSettings").SetAttributeValue("port", _portTextbox.Text);
            _selectedTemplateForDisplay.Element("RemoteInvoiceSettings").SetAttributeValue("username", _usernameTextbox.Text);
            _selectedTemplateForDisplay.Element("RemoteInvoiceSettings").SetAttributeValue("password", _passwordTextbox.Text);
            _selectedTemplateForDisplay.Element("RemoteInvoiceSettings").SetAttributeValue("keyfileLocation", _keyfileLocationTextbox.Text);
            _selectedTemplateForDisplay.Element("RemoteInvoiceSettings").SetAttributeValue("InvoiceFileCustomerPrefix", _invoiceFilePrefixTextBox.Text);
            _selectedTemplateForDisplay.Element("RemoteInvoiceSettings").SetAttributeValue("RemoteFolder", _remoteFolderTextbox.Text);

            _selectedTemplateForDisplay.Attribute("HasMasterRecord").Value = _hasMasterCheckBox.Checked ? "1" : "0";
            _selectedTemplateForDisplay.Element("MasterRow").SetAttributeValue("RecordTypePostion", _masterRecordPositionNumericUpDown.Value);
            _selectedTemplateForDisplay.Element("MasterRow").SetAttributeValue("RecordTypeIdentifier", _masterRecordIdentifierTextBox.Text);

            _selectedTemplateForDisplay.Attribute("HasSummaryRecord").Value = _hasFooterCheckBox.Checked ? "1" : "0";
            _selectedTemplateForDisplay.Element("SummaryRow").SetAttributeValue("RecordTypePostion", _footerRecordPositionNumericUpDown.Value);
            _selectedTemplateForDisplay.Element("SummaryRow").SetAttributeValue("RecordTypeIdentifier", _footerRecordIdentifierTextBox.Text);

            _selectedTemplateForDisplay.Element("DetailFields").SetAttributeValue("RecordTypePostion", _detailRecordPositionNumericUpDown.Value);
            _selectedTemplateForDisplay.Element("DetailFields").SetAttributeValue("RecordTypeIdentifier", _detailRecordIdentifierTextBox.Text);


            _selectedTemplateForDisplay.Element("MasterRow").RemoveNodes();
            _selectedTemplateForDisplay.Element("DetailFields").RemoveNodes();
            _selectedTemplateForDisplay.Element("SummaryRow").RemoveNodes();

            if (_selectedTemplateForDisplay.Elements().FirstOrDefault(e => e.Name == "EachesConversion") == null)
            {
                XElement element = new XElement("EachesConversion");
                element.Add(new XAttribute("enabled", "0"));
                element.Add(new XAttribute("tag", "*"));
                _selectedTemplateForDisplay.Add(element);
            }
            _selectedTemplateForDisplay.Element("EachesConversion").SetAttributeValue("enabled", _useEachesConversionCheckbox.Checked ? "1" : "0");
            _selectedTemplateForDisplay.Element("EachesConversion").SetAttributeValue("tag", _eachesConversionTagTextBox.Text);

            XElement el;

            var fieldDefs = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>();
            foreach (var field in fieldDefs)
            {
                el = XElement.Parse(_fieldTemplate);
                el.Attribute("FieldNameId").SetValue(field.FieldNameId);

                if (field.DirectiveId != null)
                    el.Add(new XAttribute("DirectiveId", field.DirectiveId));

                el.Element("Delimited").Attribute("Position").SetValue(field.FieldPosition);

                _selectedTemplateForDisplay.Element(field.FieldLocation.ToString()).Add(new XElement(el));
            }
        }

        private bool SelectedTemplateHasEdits()
        {
            //_originalTemplate = _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").
            //                        Where(x => x.Attribute("Id").Value == _selectedTemplateForDisplay.Attribute("Id").Value).FirstOrDefault();

            if (!XNode.DeepEquals(_originalTemplate, _selectedTemplateForDisplay))
            {
                return true;
            }
            return false;
        }

        private void _sourceFolderButton_Click(object sender, System.EventArgs e)
        {
            _folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (_folderDialog.ShowDialog(this) == DialogResult.OK)
                _sourceFolderTextBox.Text = _folderDialog.SelectedPath;
        }

        private void _outputFolderButton_Click(object sender, System.EventArgs e)
        {
            _folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (_folderDialog.ShowDialog(this) == DialogResult.OK)
                _outputFolderTextBox.Text = _folderDialog.SelectedPath;
        }

        public string SelectedTemplateId
        {
            set
            {
                foreach (var item in _templatesListBox.Items)
                {
                    var it = (TemplateListItem)item;
                    if (it.Id == value)
                    {
                        _templatesListBox.SelectedItem = it;
                        break;
                    }
                }
            }
        }

        private void _hasMasterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _masterRecordIdentifierTextBox.Enabled = _masterRecordPositionNumericUpDown.Enabled = _hasMasterCheckBox.Checked;
        }

        private void _hasFooterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _footerRecordIdentifierTextBox.Enabled = _footerRecordPositionNumericUpDown.Enabled = _hasFooterCheckBox.Checked;
        }

        private void _newTemplate_Click(object sender, EventArgs e)
        {
            var newTemplate = new XElement(_invoiceImportTemplates.Root.Element("Templates").Descendants("Template").First());
            int maxId = _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Max(x => int.Parse(x.Attribute("Id").Value));

            newTemplate.Attribute("Id").SetValue(maxId + 1);
            newTemplate.Attribute("Name").SetValue("New Template");
            newTemplate.Attribute("SourceFolder").SetValue("");
            newTemplate.Attribute("OutputFolder").SetValue(""); ;
            newTemplate.Attribute("Delimiter").SetValue(","); ;
            newTemplate.Attribute("LbProcessingType").SetValue("3");
            newTemplate.Attribute("HasHeaderRecord").SetValue("0");
            newTemplate.Attribute("HasMasterRecord").SetValue("0");
            newTemplate.Attribute("HasSummaryRecord").SetValue("0"); ;

            newTemplate.Element("MasterRow").RemoveNodes();
            newTemplate.Element("DetailFields").RemoveNodes();
            newTemplate.Element("SummaryRow").RemoveNodes();

            XElement el = XElement.Parse(_fieldTemplate);

            var fieldDefs = _invoiceImportTemplates.Root.Element("Definitions").Element("FieldNames").Descendants("FieldName");
            foreach (var field in fieldDefs)
            {
                el.Attribute("FieldNameId").SetValue(field.Attribute("Id").Value);
                el.Element("Delimited").Attribute("Position").SetValue("0");

                newTemplate.Element(FieldRecordLocation.DetailFields.ToString()).Add(new XElement(el));
            }

            _invoiceImportTemplates.Root.Element("Templates").Add(newTemplate);
            SaveTemplates();
            LoadTemplates();
        }

        private void TemplateEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SelectedTemplateHasEdits())
            {
                var result = MessageBox.Show(this,
                                string.Format("The selected template '{0}' has changed.\nSave changes before closing?", _selectedTemplateForDisplay.Attribute("Name").Value),
                                Text,
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        SaveTemplates();
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void _useEachesConversionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            _eachesConversionTagTextBox.Enabled = _useEachesConversionCheckbox.Checked;
        }
    }
}
