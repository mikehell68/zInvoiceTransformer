using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using zInvoiceTransformer.Comms;
using ZinvoiceTransformer.XmlHelpers;
using ZinvoiceTransformer.XmlModels;

namespace zInvoiceTransformer
{
    public partial class TemplateEditor : Form
    {
        static InvoiceImportTemplates _invoiceImportTemplates;
        InvoiceImportTemplatesTemplate _selectedTemplateForDisplay;
        InvoiceImportTemplatesTemplate _originalTemplate;
        readonly InvoiceTemplateModel _invoiceTemplateModel;
        bool _isInitialLoad;
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
            _invoiceImportTemplates = _invoiceTemplateModel.ImportTemplates;

            var allTemplateListItems = _invoiceImportTemplates.Templates.Select(x => new TemplateListItem { Id = x.Id.ToString(), Name = x.Name, IsInUse = x.Active == 1 }).ToArray();
            _templatesListBox.Items.Clear();
            _templatesListBox.Items.AddRange(allTemplateListItems);

            if (string.IsNullOrEmpty(selectedTemplateId) && _invoiceTemplateModel.SelectedTemplate != null)
                SelectedTemplateId = _invoiceTemplateModel.SelectedTemplate.Id.ToString();
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
                    $"The selected template '{_selectedTemplateForDisplay.Name}' has changed.\nSave changes?",
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
                        _templatesListBox.SelectedItem = _templatesListBox.Items.Cast<TemplateListItem>().First(t => t.Id == _selectedTemplateForDisplay.Id.ToString());
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

                _originalTemplate = _invoiceImportTemplates.Templates.FirstOrDefault(x => x.Id.ToString() == item.Id);
                CreateRemoteInvoiceSettingsElementIfRequired();
                _selectedTemplateForDisplay =  _originalTemplate.DeepClone(); 

                _nameTextBox.Text = _selectedTemplateForDisplay.Name;
                _descriptionTextBox.Text = _selectedTemplateForDisplay.Description;
                _activeCheckBox.Checked = _selectedTemplateForDisplay.Active == 1;
                _sourceFolderTextBox.Text = _selectedTemplateForDisplay.SourceFolder;
                _outputFolderTextBox.Text = _selectedTemplateForDisplay.OutputFolder;
                _fieldDelimiterTextBox.Text = _selectedTemplateForDisplay.Delimiter;
                _lbProcessingTypeComboBox.SelectedIndex = SetWeightProcessingRule(_selectedTemplateForDisplay.LbProcessingType.ToString());
                _hasHeaderCheckBox.Checked = _selectedTemplateForDisplay.HasHeaderRecord == 1;
                _hasMasterCheckBox.Checked = _selectedTemplateForDisplay.HasMasterRecord == 1;
                _hasFooterCheckBox.Checked = _selectedTemplateForDisplay.HasSummaryRecord == 1;
                _uomCaseSymbolTextBox.Text = _selectedTemplateForDisplay.UoMCase;
                _uomEachSymbolTextBox.Text = _selectedTemplateForDisplay.UoMEach;
                _uomWeightSymbolTextBox.Text = _selectedTemplateForDisplay.UoMWeight;

                if (_hasMasterCheckBox.Checked)
                {
                    _masterRecordIdentifierTextBox.Text = _selectedTemplateForDisplay.MasterRow.RecordTypeIdentifier;
                    _masterRecordPositionNumericUpDown.Value = _selectedTemplateForDisplay.MasterRow.RecordTypePostion;
                }
                else
                {
                    _masterRecordIdentifierTextBox.Text = "";
                    _masterRecordPositionNumericUpDown.Value = -1;
                }

                _detailRecordIdentifierTextBox.Text = _selectedTemplateForDisplay.DetailFields.RecordTypeIdentifier;
                _detailRecordPositionNumericUpDown.Value = _selectedTemplateForDisplay.DetailFields.RecordTypePostion;

                if (_hasFooterCheckBox.Checked)
                {
                    _footerRecordIdentifierTextBox.Text = _selectedTemplateForDisplay.SummaryRow.RecordTypeIdentifier;
                    _footerRecordPositionNumericUpDown.Value = _selectedTemplateForDisplay.SummaryRow.RecordTypePostion;
                }
                else
                {
                    _footerRecordIdentifierTextBox.Text = "";
                    _footerRecordPositionNumericUpDown.Value = -1;
                }

                var fieldNameDefinitions = _invoiceImportTemplates.Definitions.FieldNames;
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.MasterRow));
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.DetailFields));
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.SummaryRow));

                //var noOfmasterFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.MasterRow);
                //var noOfDetailFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.DetailFields);
                //var noOfFooterFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.SummaryRow);

                var eachesConversionElement = _selectedTemplateForDisplay.EachesConversion;
                _useEachesConversionCheckbox.Checked = eachesConversionElement != null && eachesConversionElement.enabled == 1;
                _eachesConversionTagTextBox.Text = eachesConversionElement != null ? eachesConversionElement.tag : string.Empty;
                _eachesConversionTagTextBox.Enabled = _useEachesConversionCheckbox.Checked;
                
                var transferProtocols = _invoiceImportTemplates.Definitions
                    .RemoteTransferProtocolTypes
                    .Select(el => new { id = Convert.ToInt32(el.Id), name = el.Name }).ToArray();

                _protocolTypeComboBox.DataSource = transferProtocols;
                _protocolTypeComboBox.DisplayMember = "name";
                _protocolTypeComboBox.ValueMember = "id";
                _protocolTypeComboBox.SelectedValue = Convert.ToInt32(_selectedTemplateForDisplay.RemoteInvoiceSettings?.RemoteTransferProtocolTypeId);

                _urlTextbox.Text = _selectedTemplateForDisplay.RemoteInvoiceSettings?.url;
                _portTextbox.Text = _selectedTemplateForDisplay.RemoteInvoiceSettings?.port.ToString();
                _usernameTextbox.Text = _selectedTemplateForDisplay.RemoteInvoiceSettings?.username;
                _passwordTextbox.Text = _selectedTemplateForDisplay.RemoteInvoiceSettings?.password;
                _keyfileLocationTextbox.Text = _selectedTemplateForDisplay.RemoteInvoiceSettings?.keyfileLocation;
                _invoiceFilePrefixTextBox.Text = _selectedTemplateForDisplay.RemoteInvoiceSettings?.InvoiceFileCustomerPrefix;
                _remoteFolderTextbox.Text = _selectedTemplateForDisplay.RemoteInvoiceSettings?.RemoteFolder;
            }

            _templateFieldDefinitionsFlowLayoutPanel.ResumeLayout();
        }

        private void CreateRemoteInvoiceSettingsElementIfRequired()
        {
            if (_originalTemplate.RemoteInvoiceSettings == null)
                _originalTemplate.RemoteInvoiceSettings = new InvoiceImportTemplatesTemplateRemoteInvoiceSettings
                {
                    InvoiceFileCustomerPrefix = "", 
                    keyfileLocation = "", 
                    password = "", 
                    port = 0, 
                    RemoteFolder = "",
                    RemoteTransferProtocolTypeId = 0, 
                    url = "", 
                    username = ""
                };
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

        private TemplateFieldDefinition[] CreateFieldDisplayItems(InvoiceImportTemplatesDefinitionsFieldName[] fieldNameDefinitions, FieldRecordLocation fieldRecordLocation)
        {
            var templateFieldDefinitions = new List<TemplateFieldDefinition>();
            
            foreach (var fieldElement in _selectedTemplateForDisplay..Element(fieldRecordLocation.ToString()).Descendants("Field"))
            {
                object field = fieldElement;
                var fieldName = fieldNameDefinitions.FirstOrDefault(f => f.Id == ((InvoiceImportTemplatesTemplateMasterRowField)field).FieldNameId);

                int? directiveId = null;
                if(field.Attribute("DirectiveId") != null)
                    directiveId = int.Parse(field.Attribute("DirectiveId").);
                                
                templateFieldDefinitions.Add(new TemplateFieldDefinition(fieldName.Attribute("Name").,
                                                                         int.Parse(fieldName.Attribute("Id").),
                                                                         int.Parse(field.Element("Delimited").Attribute("Position").), 
                                                                         (int)fieldRecordLocation,
                                                                         directiveId));
            }
            return templateFieldDefinitions.ToArray();
        }
        
        private void SaveTemplates()
        {
            _originalTemplate = _selectedTemplateForDisplay.DeepClone();
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

                if(MessageBox.Show(this, $"Delete selected template '{item.Name}'", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _invoiceImportTemplates.Templates.ToList().RemoveAll(t => t.Id.ToString() == item.Id);
                    //_invoiceImportTemplates.Templates.FirstOrDefault(x => x.Id.ToString() == item.Id).Remove();
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
            _folderDialog.Description = "Select the location of the keyfile required for this service.";
            _folderDialog.SelectedPath = Directory.Exists(_keyfileLocationTextbox.Text) ? 
                                        _keyfileLocationTextbox.Text : 
                                        Environment.SpecialFolder.Desktop.ToString();
            if (_folderDialog.ShowDialog(this) == DialogResult.OK)
                _keyfileLocationTextbox.Text = _folderDialog.SelectedPath;
        }

        private void _testRemoteTransferProtocolButton_Click(object sender, System.EventArgs e)
        {
            // TODO: Hook up server connection
            //_invoiceTemplateModel.GetSelectedTemplateConnectionInfo();
            var rc = RemoteConnectionFactory.Build((int)_protocolTypeComboBox.SelectedValue);
            rc.RemoteConnectionInfo = _invoiceTemplateModel.GetSelectedTemplateConnectionInfo();
            var connected = rc.CheckConnection();

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

            _selectedTemplateForDisplay.Name = _nameTextBox.Text;
            _selectedTemplateForDisplay.Description = _descriptionTextBox.Text;
            _selectedTemplateForDisplay.Active = (byte)(_activeCheckBox.Checked ? 1 : 0);
            _selectedTemplateForDisplay.SourceFolder = _sourceFolderTextBox.Text;
            _selectedTemplateForDisplay.OutputFolder = _outputFolderTextBox.Text;
            _selectedTemplateForDisplay.Delimiter = _fieldDelimiterTextBox.Text;
            _selectedTemplateForDisplay.LbProcessingType = Convert.ToByte(GetWeightProcessingRule(_lbProcessingTypeComboBox.SelectedIndex));
            _selectedTemplateForDisplay.HasHeaderRecord = (byte)(_hasHeaderCheckBox.Checked ? 1 : 0);

            _selectedTemplateForDisplay.RemoteInvoiceSettings.RemoteTransferProtocolTypeId = (byte)_protocolTypeComboBox.SelectedValue;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.url = _urlTextbox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.port = Convert.ToInt32(_portTextbox.Text);
            _selectedTemplateForDisplay.RemoteInvoiceSettings.username = _usernameTextbox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.password = _passwordTextbox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.keyfileLocation = _keyfileLocationTextbox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.InvoiceFileCustomerPrefix = _invoiceFilePrefixTextBox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.RemoteFolder = _remoteFolderTextbox.Text;

            _selectedTemplateForDisplay.HasMasterRecord = (byte)(_hasMasterCheckBox.Checked ? 1 : 0);
            _selectedTemplateForDisplay.MasterRow.RecordTypePostion = (sbyte)_masterRecordPositionNumericUpDown.Value;
            _selectedTemplateForDisplay.MasterRow.RecordTypeIdentifier = _masterRecordIdentifierTextBox.Text;

            _selectedTemplateForDisplay.HasSummaryRecord = (byte)(_hasFooterCheckBox.Checked ? 1 : 0);
            _selectedTemplateForDisplay.SummaryRow.RecordTypePostion = (sbyte)_footerRecordPositionNumericUpDown.Value;
            _selectedTemplateForDisplay.SummaryRow.RecordTypeIdentifier = _footerRecordIdentifierTextBox.Text;

            _selectedTemplateForDisplay.DetailFields.RecordTypePostion = (sbyte)_detailRecordPositionNumericUpDown.Value;
            _selectedTemplateForDisplay.DetailFields.RecordTypeIdentifier = _detailRecordIdentifierTextBox.Text;

            //????
            _selectedTemplateForDisplay.MasterRow.RemoveNodes();
            _selectedTemplateForDisplay.DetailFields.RemoveNodes();
            _selectedTemplateForDisplay.SummaryRow.RemoveNodes();

            if (_selectedTemplateForDisplay.EachesConversion == null)
            {
                _selectedTemplateForDisplay.EachesConversion = new InvoiceImportTemplatesTemplateEachesConversion
                {
                    enabled = 0,
                    tag = "*"
                };
            }
            _selectedTemplateForDisplay.EachesConversion.enabled = (byte)(_useEachesConversionCheckbox.Checked ? 1 : 0);
            _selectedTemplateForDisplay.EachesConversion.tag = _eachesConversionTagTextBox.Text;

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
            int maxId = _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Max(x => int.Parse(x.Attribute("Id").));

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
                el.Attribute("FieldNameId").SetValue(field.Attribute("Id").);
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
                                string.Format("The selected template '{0}' has changed.\nSave changes before closing?", _selectedTemplateForDisplay.Attribute("Name").),
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
