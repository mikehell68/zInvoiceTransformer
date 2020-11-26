using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using System.IO;
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

        public TemplateEditor(InvoiceTemplateModel invoiceTemplateModel)
        {
            InitializeComponent();
            _invoiceTemplateModel = invoiceTemplateModel;
            _templatesListBox.SelectedIndexChanged += _templatesListBoxSelectionChanged;
            LoadTemplates();
            InitialiseBindings();

            _isInitialLoad = false;
        }

        void InitialiseBindings()
        {
            _nameTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateForDisplay, "Name", false, DataSourceUpdateMode.Never));
            _descriptionTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateForDisplay, "Description", false, DataSourceUpdateMode.Never));
            _activeCheckBox.DataBindings.Add(new Binding("Checked", _selectedTemplateForDisplay, "Active", false,
                DataSourceUpdateMode.Never));
            _sourceFolderTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateForDisplay, "SourceFolder", false, DataSourceUpdateMode.Never));
            _outputFolderTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateForDisplay, "OutputFolder", false, DataSourceUpdateMode.Never));
            _fieldDelimiterTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateForDisplay, "Delimiter", false, DataSourceUpdateMode.Never));

        }

        private void LoadTemplates(string selectedTemplateId = null)
        {
            _invoiceImportTemplates = _invoiceTemplateModel.ImportTemplates;

            var allTemplateListItems = _invoiceImportTemplates.Templates.Select(x => new TemplateListItem { Id = x.Id.ToString(), Name = x.Name, IsInUse = x.Active}).ToArray();
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

                //_nameTextBox.Text = _selectedTemplateForDisplay.Name;
                //_descriptionTextBox.Text = _selectedTemplateForDisplay.Description;
                //_activeCheckBox.Checked = _selectedTemplateForDisplay.Active;
                //_sourceFolderTextBox.Text = _selectedTemplateForDisplay.SourceFolder;
                //_outputFolderTextBox.Text = _selectedTemplateForDisplay.OutputFolder;
                //_fieldDelimiterTextBox.Text = _selectedTemplateForDisplay.Delimiter;
                _lbProcessingTypeComboBox.SelectedIndex = SetWeightProcessingRule(_selectedTemplateForDisplay.LbProcessingType.ToString());
                _hasHeaderCheckBox.Checked = _selectedTemplateForDisplay.HasHeaderRecord;
                _hasMasterCheckBox.Checked = _selectedTemplateForDisplay.HasMasterRecord;
                _hasFooterCheckBox.Checked = _selectedTemplateForDisplay.HasSummaryRecord;
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

            switch (fieldRecordLocation)
            {
                case FieldRecordLocation.MasterRow:
                    if (_selectedTemplateForDisplay.MasterRow.Field != null)
                        foreach (var fieldElement in _selectedTemplateForDisplay.MasterRow.Field)
                        {
                            var field = fieldElement;
                            var fieldName = fieldNameDefinitions.FirstOrDefault(f => f.Id == field.FieldNameId);
                            int? directiveId = field.DirectiveIdSpecified ? field.DirectiveId : (int?) null;

                            templateFieldDefinitions.Add(new TemplateFieldDefinition(
                                fieldName.Name,
                                fieldName.Id,
                                field.Delimited.Position,
                                (int) fieldRecordLocation,
                                directiveId));
                        }

                    break;
                
                case FieldRecordLocation.DetailFields:
                    if (_selectedTemplateForDisplay.DetailFields.Field != null)
                        foreach (var fieldElement in _selectedTemplateForDisplay.DetailFields.Field)
                        {
                            var field = fieldElement;
                            var fieldName = fieldNameDefinitions.FirstOrDefault(f => f.Id == field.FieldNameId);
                            int? directiveId = field.DirectiveIdSpecified ? field.DirectiveId : (int?) null;

                            templateFieldDefinitions.Add(new TemplateFieldDefinition(
                                fieldName.Name,
                                fieldName.Id,
                                field.Delimited.Position,
                                (int) fieldRecordLocation,
                                directiveId));
                        }

                    break;

                case FieldRecordLocation.SummaryRow:
                    if (_selectedTemplateForDisplay.SummaryRow.Field != null)
                        foreach (var fieldElement in _selectedTemplateForDisplay.SummaryRow.Field)
                        {
                            var field = fieldElement;
                            var fieldName = fieldNameDefinitions.FirstOrDefault(f => f.Id == field.FieldNameId);
                            int? directiveId = field.DirectiveIdSpecified ? field.DirectiveId : (int?) null;

                            templateFieldDefinitions.Add(new TemplateFieldDefinition(
                                fieldName.Name,
                                fieldName.Id,
                                field.Delimited.Position,
                                (int) fieldRecordLocation,
                                directiveId));
                        }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldRecordLocation), fieldRecordLocation, null);
            }
            
            return templateFieldDefinitions.ToArray();
        }
        
        private void SaveTemplates()
        {
            _originalTemplate = _selectedTemplateForDisplay.DeepClone();
            _invoiceImportTemplates.Save<InvoiceImportTemplates>(InvoiceTemplateModel.InvoiceImportTemplatePath);
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
            _selectedTemplateForDisplay.Active = _activeCheckBox.Checked;
            _selectedTemplateForDisplay.SourceFolder = _sourceFolderTextBox.Text;
            _selectedTemplateForDisplay.OutputFolder = _outputFolderTextBox.Text;
            _selectedTemplateForDisplay.Delimiter = _fieldDelimiterTextBox.Text;
            _selectedTemplateForDisplay.LbProcessingType = Convert.ToByte(GetWeightProcessingRule(_lbProcessingTypeComboBox.SelectedIndex));
            _selectedTemplateForDisplay.HasHeaderRecord = _hasHeaderCheckBox.Checked;

            _selectedTemplateForDisplay.RemoteInvoiceSettings.RemoteTransferProtocolTypeId = Convert.ToByte(_protocolTypeComboBox.SelectedValue);
            _selectedTemplateForDisplay.RemoteInvoiceSettings.url = _urlTextbox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.port = Convert.ToInt32(_portTextbox.Text);
            _selectedTemplateForDisplay.RemoteInvoiceSettings.username = _usernameTextbox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.password = _passwordTextbox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.keyfileLocation = _keyfileLocationTextbox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.InvoiceFileCustomerPrefix = _invoiceFilePrefixTextBox.Text;
            _selectedTemplateForDisplay.RemoteInvoiceSettings.RemoteFolder = _remoteFolderTextbox.Text;

            _selectedTemplateForDisplay.HasMasterRecord =_hasMasterCheckBox.Checked;
            _selectedTemplateForDisplay.MasterRow.RecordTypePostion = (sbyte)_masterRecordPositionNumericUpDown.Value;
            _selectedTemplateForDisplay.MasterRow.RecordTypeIdentifier = _masterRecordIdentifierTextBox.Text;

            _selectedTemplateForDisplay.HasSummaryRecord = _hasFooterCheckBox.Checked;
            _selectedTemplateForDisplay.SummaryRow.RecordTypePostion = (sbyte)_footerRecordPositionNumericUpDown.Value;
            _selectedTemplateForDisplay.SummaryRow.RecordTypeIdentifier = _footerRecordIdentifierTextBox.Text;

            _selectedTemplateForDisplay.DetailFields.RecordTypePostion = (sbyte)_detailRecordPositionNumericUpDown.Value;
            _selectedTemplateForDisplay.DetailFields.RecordTypeIdentifier = _detailRecordIdentifierTextBox.Text;

            //????
            _selectedTemplateForDisplay.MasterRow = new InvoiceImportTemplatesTemplateMasterRow();
            _selectedTemplateForDisplay.DetailFields = new InvoiceImportTemplatesTemplateDetailFields();
            _selectedTemplateForDisplay.SummaryRow = new InvoiceImportTemplatesTemplateSummaryRow();

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

            var fieldDefs = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>();
            foreach (var field in fieldDefs)
            {
                switch (field.FieldLocation)
                {
                    case FieldRecordLocation.MasterRow:
                        _selectedTemplateForDisplay.MasterRow.Field?.ToList().Add(
                            new InvoiceImportTemplatesTemplateMasterRowField
                            {
                                FieldNameId = (byte) field.FieldNameId,
                                DirectiveId = (byte) (field.DirectiveId ?? new byte()),
                                Delimited = new InvoiceImportTemplatesTemplateMasterRowFieldDelimited
                                {
                                    Position = (byte) field.FieldPosition
                                }
                            });
                        break;
                    
                    case FieldRecordLocation.DetailFields:
                        _selectedTemplateForDisplay.DetailFields.Field?.ToList().
                            Add(new InvoiceImportTemplatesTemplateDetailFieldsField
                            {
                                FieldNameId = (byte)field.FieldNameId,
                                DirectiveId = (byte) (field.DirectiveId ?? new byte()),
                                Delimited = new InvoiceImportTemplatesTemplateDetailFieldsFieldDelimited
                                {
                                    Position = (byte) field.FieldPosition
                                }

                            });
                        break;
                    
                    case FieldRecordLocation.SummaryRow:
                        _selectedTemplateForDisplay.SummaryRow.Field?.ToList().
                            Add(new InvoiceImportTemplatesTemplateSummaryRowField()
                            {
                                FieldNameId = (byte)field.FieldNameId,
                                DirectiveId = (byte)(field.DirectiveId ?? new byte()),
                                Delimited = new InvoiceImportTemplatesTemplateSummaryRowFieldDelimited()
                                {
                                    Position = (byte)field.FieldPosition
                                }
                            });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var templateToUpdateIndex =_invoiceImportTemplates.Templates.ToList()
                .IndexOf(_invoiceImportTemplates.Templates.FirstOrDefault(t => t.Id == _selectedTemplateForDisplay.Id));

            _invoiceImportTemplates.Templates[templateToUpdateIndex] = _selectedTemplateForDisplay;
        }

        private bool SelectedTemplateHasEdits()
        {
            //_originalTemplate = _invoiceImportTemplates.Templates.FirstOrDefault(x => x.Id == _selectedTemplateForDisplay.Id);

            //if (!XNode.DeepEquals(_originalTemplate, _selectedTemplateForDisplay))
            //{
            //    return true;
            //}

            if(_selectedTemplateForDisplay == null )
                return false;

            return true;
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
            var newTemplate = new InvoiceImportTemplatesTemplate();
            int maxId = _invoiceImportTemplates.Templates.Max(x => x.Id);

            newTemplate.Id = (byte) (maxId + 1);
            newTemplate.Name = "New Template";
            newTemplate.SourceFolder = "";
            newTemplate.OutputFolder = "";
            newTemplate.Delimiter = ",";
            newTemplate.LbProcessingType = 3;
            newTemplate.HasHeaderRecord = false;
            newTemplate.HasMasterRecord = false;
            newTemplate.HasSummaryRecord = false;

            newTemplate.MasterRow = new InvoiceImportTemplatesTemplateMasterRow();
            newTemplate.DetailFields = new InvoiceImportTemplatesTemplateDetailFields();
            newTemplate.SummaryRow = new InvoiceImportTemplatesTemplateSummaryRow();

            //XElement el = XElement.Parse(_fieldTemplate);
            var el = new InvoiceImportTemplatesTemplateDetailFieldsField();
            var fieldDefs = _invoiceImportTemplates.Definitions.FieldNames;
            foreach (var field in fieldDefs)
            {
                el.FieldNameId = field.Id;
                el.Delimited.Position = 0;

                newTemplate.DetailFields.Field.ToList().Add(el);
            }
            _invoiceImportTemplates.Templates.ToList().Add(newTemplate);
            //_invoiceImportTemplates.Root.Element("Templates").Add(newTemplate);
            SaveTemplates();
            LoadTemplates();
        }

        private void TemplateEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SelectedTemplateHasEdits())
            {
                var result = MessageBox.Show(this,
                    $"The selected template '{_selectedTemplateForDisplay.Name}' has changed.\nSave changes before closing?",
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
