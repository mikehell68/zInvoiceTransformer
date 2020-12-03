using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using System.IO;
using LogThis;
using zInvoiceTransformer.Comms;
using ZinvoiceTransformer.XmlHelpers;
using ZinvoiceTransformer.XmlModels;

namespace zInvoiceTransformer
{
    public partial class TemplateEditor : Form
    {
        InvoiceImportTemplatesTemplate _selectedTemplateClone;
        InvoiceImportTemplatesTemplate _originalTemplate;
        readonly InvoiceTemplateModel _invoiceTemplateModel;
        
        public TemplateEditor(InvoiceTemplateModel invoiceTemplateModel)
        {
            InitializeComponent();
            _invoiceTemplateModel = invoiceTemplateModel;
            InitializeTemplateModels(_invoiceTemplateModel.SelectedTemplate.Id);
            InitialiseBindings();
            _templatesListBox.SelectedIndexChanged += _templatesListBoxSelectionChanged;
            PopulateTemplateListBox();
            PopulateUiWithSelectedTemplateClone();
            SetSelectedTemplate(_invoiceTemplateModel.SelectedTemplate.Id);
        }

        private void InitializeTemplateModels(int templateId)
        {
            _originalTemplate = _invoiceTemplateModel.GetTemplate(templateId);
            CreateRemoteInvoiceSettingsElementIfRequired();
            _selectedTemplateClone = AnyClone.Cloner.Clone(_originalTemplate);
        }

        void InitialiseBindings()
        {
            _nameTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateClone, "Name", false, DataSourceUpdateMode.Never));
            _descriptionTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateClone, "Description", false, DataSourceUpdateMode.Never));
            _activeCheckBox.DataBindings.Add(new Binding("Checked", _selectedTemplateClone, "Active", false,
                DataSourceUpdateMode.Never));
            _sourceFolderTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateClone, "SourceFolder", false, DataSourceUpdateMode.Never));
            _outputFolderTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateClone, "OutputFolder", false, DataSourceUpdateMode.Never));
            _fieldDelimiterTextBox.DataBindings.Add(new Binding("Text", _selectedTemplateClone, "Delimiter", false, DataSourceUpdateMode.Never));

        }

        private void PopulateTemplateListBox()
        {
            var allTemplateListItems = _invoiceTemplateModel.ImportTemplates.Templates.Select(x => new TemplateListItem { Id = x.Id.ToString(), Name = x.Name, IsInUse = x.Active}).ToArray();
            _templatesListBox.Items.Clear();
            _templatesListBox.Items.AddRange(allTemplateListItems);
        }

        void SetSelectedTemplate(int templateId)
        {
            _templatesListBox.SelectedItem = _templatesListBox.Items.OfType<TemplateListItem>()
                .FirstOrDefault(ti => ti.Id == templateId.ToString());
        }

        void _templatesListBoxSelectionChanged(object sender, System.EventArgs e)
        {
            SaveUiChangesToSelectedTemplate();
            
            if (SelectedTemplateCloneHasEdits())
            {
                var result = MessageBox.Show(this,
                    $"The selected template '{_selectedTemplateClone.Name}' has changed.\nSave changes?",
                                Text,
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        SaveSelectedTemplateToTemplateList();
                        SaveTemplatesToFile();
                        InitializeTemplateModels(Convert.ToInt32(((TemplateListItem)_templatesListBox.SelectedItem).Id));
                        break;
                    case DialogResult.No:
                        InitializeTemplateModels(Convert.ToInt32(((TemplateListItem)_templatesListBox.SelectedItem).Id));
                        break;
                    case DialogResult.Cancel:
                    default:
                        _templatesListBox.SelectedIndexChanged -= _templatesListBoxSelectionChanged;
                        SetSelectedTemplate(_selectedTemplateClone.Id);
                        _templatesListBox.SelectedIndexChanged += _templatesListBoxSelectionChanged;
                        return;
                }
                PopulateTemplateListBox();
                PopulateUiWithSelectedTemplateClone();
                SetSelectedTemplate(_selectedTemplateClone.Id);
            }
            else
            {
                InitializeTemplateModels(Convert.ToInt32(((TemplateListItem)_templatesListBox.SelectedItem).Id));
                PopulateUiWithSelectedTemplateClone();
            }
        }

        private void PopulateUiWithSelectedTemplateClone()
        {
            _templateFieldDefinitionsFlowLayoutPanel.SuspendLayout();

            _templateFieldDefinitionsFlowLayoutPanel.Controls.Clear();

            if (_selectedTemplateClone != null)
            {
                _nameTextBox.Text = _selectedTemplateClone.Name;
                _descriptionTextBox.Text = _selectedTemplateClone.Description;
                _activeCheckBox.Checked = _selectedTemplateClone.Active;
                _sourceFolderTextBox.Text = _selectedTemplateClone.SourceFolder;
                _outputFolderTextBox.Text = _selectedTemplateClone.OutputFolder;
                _fieldDelimiterTextBox.Text = _selectedTemplateClone.Delimiter;
                _lbProcessingTypeComboBox.SelectedIndex = SetWeightProcessingRule(_selectedTemplateClone.LbProcessingType.ToString());
                _hasHeaderCheckBox.Checked = _selectedTemplateClone.HasHeaderRecord;
                _hasMasterCheckBox.Checked = _selectedTemplateClone.HasMasterRecord;
                _hasFooterCheckBox.Checked = _selectedTemplateClone.HasSummaryRecord;
                _uomCaseSymbolTextBox.Text = _selectedTemplateClone.UoMCase;
                _uomEachSymbolTextBox.Text = _selectedTemplateClone.UoMEach;
                _uomWeightSymbolTextBox.Text = _selectedTemplateClone.UoMWeight;

                if (_hasMasterCheckBox.Checked)
                {
                    _masterRecordIdentifierTextBox.Text = _selectedTemplateClone.MasterRow.RecordTypeIdentifier;
                    _masterRecordPositionNumericUpDown.Value = _selectedTemplateClone.MasterRow.RecordTypePostion;
                }
                else
                {
                    _masterRecordIdentifierTextBox.Text = "";
                    _masterRecordPositionNumericUpDown.Value = -1;
                }

                _detailRecordIdentifierTextBox.Text = _selectedTemplateClone.DetailFields.RecordTypeIdentifier;
                _detailRecordPositionNumericUpDown.Value = _selectedTemplateClone.DetailFields.RecordTypePostion;

                if (_hasFooterCheckBox.Checked)
                {
                    _footerRecordIdentifierTextBox.Text = _selectedTemplateClone.SummaryRow.RecordTypeIdentifier;
                    _footerRecordPositionNumericUpDown.Value = _selectedTemplateClone.SummaryRow.RecordTypePostion;
                }
                else
                {
                    _footerRecordIdentifierTextBox.Text = "";
                    _footerRecordPositionNumericUpDown.Value = -1;
                }

                var fieldNameDefinitions = _invoiceTemplateModel.ImportTemplates.Definitions.FieldNames;
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.MasterRow));
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.DetailFields));
                _templateFieldDefinitionsFlowLayoutPanel.Controls.AddRange(CreateFieldDisplayItems(fieldNameDefinitions, FieldRecordLocation.SummaryRow));

                //var noOfmasterFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.MasterRow);
                //var noOfDetailFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.DetailFields);
                //var noOfFooterFields = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>().Count(x => x.FieldLocation == FieldRecordLocation.SummaryRow);

                var eachesConversionElement = _selectedTemplateClone.EachesConversion;
                _useEachesConversionCheckbox.Checked = eachesConversionElement != null && eachesConversionElement.enabled == 1;
                _eachesConversionTagTextBox.Text = eachesConversionElement != null ? eachesConversionElement.tag : string.Empty;
                _eachesConversionTagTextBox.Enabled = _useEachesConversionCheckbox.Checked;
                
                var transferProtocols = _invoiceTemplateModel.ImportTemplates.Definitions
                    .RemoteTransferProtocolTypes
                    .Select(el => new { id = Convert.ToInt32(el.Id), name = el.Name }).ToArray();

                _protocolTypeComboBox.DataSource = transferProtocols;
                _protocolTypeComboBox.DisplayMember = "name";
                _protocolTypeComboBox.ValueMember = "id";
                _protocolTypeComboBox.SelectedValue = Convert.ToInt32(_selectedTemplateClone.RemoteInvoiceSettings?.RemoteTransferProtocolTypeId);

                _urlTextbox.Text = _selectedTemplateClone.RemoteInvoiceSettings?.url;
                _portTextbox.Text = _selectedTemplateClone.RemoteInvoiceSettings?.port.ToString();
                _usernameTextbox.Text = _selectedTemplateClone.RemoteInvoiceSettings?.username;
                _passwordTextbox.Text = _selectedTemplateClone.RemoteInvoiceSettings?.password;
                _keyfileLocationTextbox.Text = _selectedTemplateClone.RemoteInvoiceSettings?.keyfileLocation;
                _invoiceFilePrefixTextBox.Text = _selectedTemplateClone.RemoteInvoiceSettings?.InvoiceFileCustomerPrefix;
                _remoteFolderTextbox.Text = _selectedTemplateClone.RemoteInvoiceSettings?.RemoteFolder;
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
                    if (_selectedTemplateClone.MasterRow.Field != null)
                        foreach (var fieldElement in _selectedTemplateClone.MasterRow.Field)
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
                    if (_selectedTemplateClone.DetailFields.Field != null)
                        foreach (var fieldElement in _selectedTemplateClone.DetailFields.Field)
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
                    if (_selectedTemplateClone.SummaryRow.Field != null)
                        foreach (var fieldElement in _selectedTemplateClone.SummaryRow.Field)
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
        
        private void SaveTemplatesToFile()
        {
            _invoiceTemplateModel.ImportTemplates.Save<InvoiceImportTemplates>(InvoiceTemplateModel.InvoiceImportTemplatePath);
        }

        private void SaveSelectedTemplateToTemplateList()
        {
            var templateToUpdateIndex = _invoiceTemplateModel.ImportTemplates.Templates.ToList()
                .IndexOf(_invoiceTemplateModel.ImportTemplates.Templates.FirstOrDefault(t => t.Id == _selectedTemplateClone.Id));

            _invoiceTemplateModel.ImportTemplates.Templates[templateToUpdateIndex] = _selectedTemplateClone;
        }

        private void _closeButton_Click(object sender, System.EventArgs e)
        {
            SaveUiChangesToSelectedTemplate();
            Close();
        }

        private void _deleteTemplateButton_Click(object sender, System.EventArgs e)
        {
            if (_templatesListBox.SelectedItem != null)
            {
                var item = (TemplateListItem)_templatesListBox.SelectedItem;

                if(MessageBox.Show(this, $"Delete selected template '{item.Name}'", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var templates = _invoiceTemplateModel.ImportTemplates.Templates.ToList();
                    templates.RemoveAll(t => t.Id == _selectedTemplateClone.Id);
                    _invoiceTemplateModel.ImportTemplates.Templates = templates.ToArray();
                    SaveTemplatesToFile();
                    InitializeTemplateModels(_invoiceTemplateModel.ImportTemplates.Templates.FirstOrDefault().Id);
                    PopulateTemplateListBox();
                    PopulateUiWithSelectedTemplateClone();
                    SetSelectedTemplate(_selectedTemplateClone.Id);
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
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                SaveUiChangesToSelectedTemplate();

                var rc = RemoteConnectionFactory.Build((int) _protocolTypeComboBox.SelectedValue);
                rc.RemoteConnectionInfo = GetSelectedTemplateConnectionInfo();
                var connected = rc.CheckConnection();

                MessageBox.Show(this, $"Connection to {_urlTextbox.Text}: " + (connected ? "ok" : "failed"),
                    "Test Connection", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogThis(ex.Message + '\n' + ex.StackTrace, eloglevel.error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void _saveButton_Click(object sender, System.EventArgs e)
        {
            if (_selectedTemplateClone == null)
                return;

            SaveUiChangesToSelectedTemplate();
            if (SelectedTemplateCloneHasEdits())
            {
                SaveSelectedTemplateToTemplateList();
                SaveTemplatesToFile();
                InitializeTemplateModels(_selectedTemplateClone.Id);
            }
        }

        private void SaveUiChangesToSelectedTemplate()
        {
            if (_selectedTemplateClone == null)
                return;

            _selectedTemplateClone.Name = _nameTextBox.Text;
            _selectedTemplateClone.Description = _descriptionTextBox.Text;
            _selectedTemplateClone.Active = _activeCheckBox.Checked;
            _selectedTemplateClone.SourceFolder = _sourceFolderTextBox.Text;
            _selectedTemplateClone.OutputFolder = _outputFolderTextBox.Text;
            _selectedTemplateClone.Delimiter = _fieldDelimiterTextBox.Text;
            _selectedTemplateClone.LbProcessingType = Convert.ToByte(GetWeightProcessingRule(_lbProcessingTypeComboBox.SelectedIndex));
            _selectedTemplateClone.HasHeaderRecord = _hasHeaderCheckBox.Checked;

            _selectedTemplateClone.RemoteInvoiceSettings.RemoteTransferProtocolTypeId = Convert.ToByte(_protocolTypeComboBox.SelectedValue);
            _selectedTemplateClone.RemoteInvoiceSettings.url = _urlTextbox.Text;
            _selectedTemplateClone.RemoteInvoiceSettings.port = Convert.ToInt32(string.IsNullOrEmpty(_portTextbox.Text) ? "0" : _portTextbox.Text) ;
            _selectedTemplateClone.RemoteInvoiceSettings.username = _usernameTextbox.Text;
            _selectedTemplateClone.RemoteInvoiceSettings.password = _passwordTextbox.Text;
            _selectedTemplateClone.RemoteInvoiceSettings.keyfileLocation = _keyfileLocationTextbox.Text;
            _selectedTemplateClone.RemoteInvoiceSettings.InvoiceFileCustomerPrefix = _invoiceFilePrefixTextBox.Text;
            _selectedTemplateClone.RemoteInvoiceSettings.RemoteFolder = _remoteFolderTextbox.Text;

            _selectedTemplateClone.HasMasterRecord =_hasMasterCheckBox.Checked;
            _selectedTemplateClone.MasterRow.RecordTypePostion = (sbyte)_masterRecordPositionNumericUpDown.Value;
            _selectedTemplateClone.MasterRow.RecordTypeIdentifier = _masterRecordIdentifierTextBox.Text;

            _selectedTemplateClone.HasSummaryRecord = _hasFooterCheckBox.Checked;
            _selectedTemplateClone.SummaryRow.RecordTypePostion = (sbyte)_footerRecordPositionNumericUpDown.Value;
            _selectedTemplateClone.SummaryRow.RecordTypeIdentifier = _footerRecordIdentifierTextBox.Text;

            _selectedTemplateClone.DetailFields.RecordTypePostion = (sbyte)_detailRecordPositionNumericUpDown.Value;
            _selectedTemplateClone.DetailFields.RecordTypeIdentifier = _detailRecordIdentifierTextBox.Text;

            if (_selectedTemplateClone.EachesConversion == null)
            {
                _selectedTemplateClone.EachesConversion = new InvoiceImportTemplatesTemplateEachesConversion
                {
                    enabled = 0,
                    tag = "*"
                };
            }
            _selectedTemplateClone.EachesConversion.enabled = (byte)(_useEachesConversionCheckbox.Checked ? 1 : 0);
            _selectedTemplateClone.EachesConversion.tag = _eachesConversionTagTextBox.Text;

            var masterFieldList = new List<InvoiceImportTemplatesTemplateMasterRowField>();
            var detailFieldList = new List<InvoiceImportTemplatesTemplateDetailFieldsField>();
            var summaryFieldList = new List<InvoiceImportTemplatesTemplateSummaryRowField>();

            var fieldDefinitions = _templateFieldDefinitionsFlowLayoutPanel.Controls.OfType<TemplateFieldDefinition>();
            foreach (var field in fieldDefinitions)
            {
                switch (field.FieldLocation)
                {
                    case FieldRecordLocation.MasterRow:
                        var masterField = _selectedTemplateClone.MasterRow.Field?.ToList()
                                      .FirstOrDefault(f => f.FieldNameId == field.FieldNameId) ?? 
                                      new InvoiceImportTemplatesTemplateMasterRowField
                                      {
                                          Delimited = new InvoiceImportTemplatesTemplateMasterRowFieldDelimited()
                                      };

                        masterField.FieldNameId = (byte)field.FieldNameId;
                        masterField.DirectiveId = (byte)(field.DirectiveId ?? new byte());
                        masterField.Delimited.Position = (byte)field.FieldPosition;

                        masterFieldList.Add(masterField);
                        
           
                        break;
                    
                    case FieldRecordLocation.DetailFields:
                        
                        var detailField = _selectedTemplateClone.DetailFields.Field?.ToList()
                                      .FirstOrDefault(f => f.FieldNameId == field.FieldNameId) ??
                                  new InvoiceImportTemplatesTemplateDetailFieldsField
                                  {
                                      Delimited = new InvoiceImportTemplatesTemplateDetailFieldsFieldDelimited()
                                  };
                        detailField.FieldNameId = (byte)field.FieldNameId;
                        detailField.DirectiveId = (byte)(field.DirectiveId ?? new byte());
                        detailField.Delimited.Position = (byte)field.FieldPosition;
                        detailFieldList.Add(detailField);
                        break;
                    
                    case FieldRecordLocation.SummaryRow:
                        
                        var summaryField = _selectedTemplateClone.SummaryRow.Field?.ToList()
                                       .FirstOrDefault(f => f.FieldNameId == field.FieldNameId) ??
                                   new InvoiceImportTemplatesTemplateSummaryRowField
                                   {
                                       Delimited = new InvoiceImportTemplatesTemplateSummaryRowFieldDelimited()
                                   };
                        summaryField.FieldNameId = (byte)field.FieldNameId;
                        summaryField.DirectiveId = (byte)(field.DirectiveId ?? new byte());
                        summaryField.Delimited.Position = (byte)field.FieldPosition;
                        
                        summaryFieldList.Add(summaryField);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            _selectedTemplateClone.MasterRow.Field = masterFieldList.Any() ? masterFieldList.ToArray() : null;
            _selectedTemplateClone.DetailFields.Field = detailFieldList.Any() ? detailFieldList.ToArray() : null;
            _selectedTemplateClone.SummaryRow.Field = summaryFieldList.Any() ? summaryFieldList.ToArray() : null;
        }

        private bool SelectedTemplateCloneHasEdits()
        {
            if(_selectedTemplateClone == null )
                return false;

            return AnyDiff.AnyDiff.Diff(_originalTemplate, _selectedTemplateClone).Count > 0;
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
            SaveUiChangesToSelectedTemplate();
            if(SelectedTemplateCloneHasEdits())
                _templatesListBoxSelectionChanged(sender, e);

            var newTemplate = new InvoiceImportTemplatesTemplate();
            int maxId = _invoiceTemplateModel.ImportTemplates.Templates.Max(x => x.Id);

            newTemplate.Id = (byte) (maxId + 1);
            newTemplate.Name = $"New Template {newTemplate.Id}";
            newTemplate.Description = "";
            newTemplate.SourceFolder = "";
            newTemplate.OutputFolder = "";
            newTemplate.Delimiter = ",";
            newTemplate.LbProcessingType = 3;
            newTemplate.HasHeaderRecord = false;
            newTemplate.HasMasterRecord = false;
            newTemplate.HasSummaryRecord = false;
            newTemplate.EachesConversion = new InvoiceImportTemplatesTemplateEachesConversion
            {
                enabled = 0, tag = "*"
            };

            newTemplate.MasterRow = new InvoiceImportTemplatesTemplateMasterRow
            {
                RecordTypeIdentifier = "",
                RecordTypePostion = -1,
                //Field = new InvoiceImportTemplatesTemplateMasterRowField[] { }
            };

            newTemplate.DetailFields = new InvoiceImportTemplatesTemplateDetailFields
            {
                RecordTypeIdentifier = "",
                RecordTypePostion = -1,
                Field = new InvoiceImportTemplatesTemplateDetailFieldsField[] { }
            };

            newTemplate.SummaryRow = new InvoiceImportTemplatesTemplateSummaryRow
            {
                RecordTypeIdentifier = "",
                RecordTypePostion = -1,
                //Field = new InvoiceImportTemplatesTemplateSummaryRowField[] { }
            };

            // create all invoice definition fields as 'detail' rows
            var detailFields = new List<InvoiceImportTemplatesTemplateDetailFieldsField>();
            var invoiceFieldDefinitions = _invoiceTemplateModel.ImportTemplates.Definitions.FieldNames;
            foreach (var definitionField in invoiceFieldDefinitions)
            {
                var detailField = new InvoiceImportTemplatesTemplateDetailFieldsField
                {
                    Delimited = new InvoiceImportTemplatesTemplateDetailFieldsFieldDelimited { Position = 0 },
                    FieldNameId = definitionField.Id
                };
                detailFields.Add(detailField);
                
            }
            newTemplate.DetailFields.Field = detailFields.ToArray();

            var templateList = _invoiceTemplateModel.ImportTemplates.Templates.ToList();
            templateList.Add(newTemplate);
            _invoiceTemplateModel.ImportTemplates.Templates = templateList.ToArray();
            
            SaveTemplatesToFile();
            InitializeTemplateModels(newTemplate.Id);
            PopulateTemplateListBox();
            PopulateUiWithSelectedTemplateClone();
            SetSelectedTemplate(newTemplate.Id);
        }

        private void TemplateEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SelectedTemplateCloneHasEdits())
            {
                var result = MessageBox.Show(this,
                    $"The selected template '{_selectedTemplateClone.Name}' has changed.\nSave changes before closing?",
                                Text,
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        SaveTemplatesToFile();
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
        public RemoteInvoiceConnectionInfo GetSelectedTemplateConnectionInfo()
        {
            try
            {
                if (_selectedTemplateClone != null)
                    return new RemoteInvoiceConnectionInfo
                    {
                        DestinationFolder = _selectedTemplateClone.SourceFolder,
                        InvoiceFilePrefix = _selectedTemplateClone.RemoteInvoiceSettings.InvoiceFileCustomerPrefix,
                        HostUrl = _selectedTemplateClone.RemoteInvoiceSettings.url,
                        Port = _selectedTemplateClone.RemoteInvoiceSettings.port,
                        Username = _selectedTemplateClone.RemoteInvoiceSettings.username,
                        Password = _selectedTemplateClone.RemoteInvoiceSettings.password,
                        RemoteFolder = _selectedTemplateClone.RemoteInvoiceSettings.RemoteFolder
                    };

                Log.LogThis("Unable to create RemoteInvoiceConnectionInfo - Selected template is null, check template exists for this supplier", eloglevel.error);
                return null;
            }
            catch (Exception e)
            {
                Log.LogThis($"Unable to create RemoteInvoiceConnectionInfo, check invoice template: {e.Message}", eloglevel.error);
                return null;
            }
        }
    }
}
