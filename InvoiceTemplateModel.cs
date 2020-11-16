using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LogThis;
using System.ComponentModel;

namespace zInvoiceTransformer
{
    public class InvoiceTemplateModel : INotifyPropertyChanged
    {
        public const string InvoiceImportTemplatePath = @"InvoiceImportTemplates.xml";
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) 
                return false;

            field = value;
            IsDirty = true;
            OnPropertyChanged(propertyName);
            return true;
        }
        
        XDocument _invoiceImportTemplates;
        public XDocument InvoiceImportTemplates
        {
            get => _invoiceImportTemplates;
            set => _invoiceImportTemplates = value;
        }

        XElement _selectedTemplate;
        public XElement SelectedTemplate
        {
            get => _selectedTemplate;
            set => _selectedTemplate = value;
        }

        string _selectedTemplateName;
        public string SelectedTemplateName
        {
            get => _selectedTemplateName;
            private set => SetField(ref _selectedTemplateName, value, "SelectedTemplateName");
        }

        string _selectedTemplateDescription;
        public string SelectedTemplateDescription
        {
            get => _selectedTemplateDescription;
            private set => SetField(ref _selectedTemplateDescription, value, "SelectedTemplateDescription");
        }

        string _importAppLocation;
        public string ImportAppLocation
        { 
            get => _importAppLocation;
            set => SetField(ref _importAppLocation, value, "ImportAppLocation");
        }

        string _importAppInvoiceFileLocation;
        public string ImportAppInvoiceFileLocation
        {
            get => _importAppInvoiceFileLocation;
            set => SetField(ref _importAppInvoiceFileLocation, value, "ImportAppInvoiceFileLocation");
        }

        public InvoiceTemplateModel()
        {
            LoadTemplates();
            _importAppLocation = _invoiceImportTemplates.Root.Element("ImportSettings").Element("ImportAppliction").Attribute("FileName").Value;
            _importAppInvoiceFileLocation = _invoiceImportTemplates.Root.Element("ImportSettings").Element("ImportAppliction").Attribute("InvoiceFileLocation").Value;
        }
        
        public void LoadTemplates()
        {
            Log.LogThis("Loading invoice templates", eloglevel.info);
            string templatesXml;
            try
            {
                templatesXml = File.ReadAllText(InvoiceImportTemplatePath);
            }
            catch (Exception ex)
            {
                Log.LogThis($"Error reading invoice templates file: {ex}", eloglevel.error);
                throw;
            }

            var templatesStringReader = new StringReader(templatesXml);
            try
            {
                _invoiceImportTemplates = XDocument.Load(templatesStringReader);
            }
            catch (Exception ex)
            {
                Log.LogThis($"Error loading xml from invoice templates file: {ex}", eloglevel.error);
                throw;
            }
            finally
            {
                templatesStringReader.Close();
            }
            Log.LogThis(
                $"{_invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Count()} invoice templates loaded ", eloglevel.info);
        }

        public TemplateListItem[] GetAllTemplatesArray()
        {
            if (_invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Count() > 0)
            {
                return _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Select(
                    template => new TemplateListItem
                                    {
                                        Id = template.Attribute("Id").Value,
                                        Name = template.Attribute("Name").Value,
                                        IsInUse = template.Attribute("Active").Value == "1"
                                    }).ToArray();
            }

            return new TemplateListItem[0];
        }

        public IEnumerable<XElement> GetAllTemplates()
        {
            if (_invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Count() > 0)
            {
                return _invoiceImportTemplates.Root.Element("Templates").Descendants("Template");
            }

            return null;
        }

        public IEnumerable<XElement> GetAllActiveTemplates()
        {
            var allActiveTemplates = GetAllTemplates();
            
            if (allActiveTemplates != null)
            {
                return GetAllTemplates().Where(t => t.Attribute("Active").Value == "1");
            }

            return new List<XElement>();
        }

        public XElement GetTemplate(string templateId)
        {
            XElement template = null;

            if (_invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Count() > 0)
            {
                template = _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Where(
                    t => t.Attribute("Id").Value == templateId).FirstOrDefault();
            }

            SelectedTemplateName = template == null ? "" : template.Attribute("Name").Value;
            SelectedTemplateDescription = template == null ? "" : template.Attribute("Description").Value;
            IsDirty = false;
            return template;
        }

        public string[] GetSelectedTemplateImportFiles()
        {
            if (SelectedTemplate == null)
                return new string[]{ "<-no supplier selected->" };

            string sourceFolder = SelectedTemplate.Attribute("SourceFolder").Value;
            var files = new List<string>
            {
                sourceFolder
            };

            if (Directory.Exists(sourceFolder))
            {
                var fileList = Directory.GetFiles(sourceFolder).Select(Path.GetFileName).ToArray();

                if (fileList.Any())
                {
                    files.AddRange(fileList);
                    return files.ToArray();
                }
                else
                    return new string[]{ sourceFolder, "<-folder empty->" };
            }
            
            return null;
        }

        public RemoteInvoiceConnectionInfo GetSelectedTemplateConnectionInfo()
        {
            try
            {
                if (SelectedTemplate != null)
                    return new RemoteInvoiceConnectionInfo
                    {
                        DestinationFolder = SelectedTemplate.Attribute("SourceFolder").Value,
                        InvoiceFilePrefix = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("InvoiceFileCustomerPrefix").Value,
                        HostUrl = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("url").Value,
                        Port = Convert.ToInt32(SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("port").Value),
                        Username = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("username").Value,
                        Password = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("password").Value,
                        RemoteFolder = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("RemoteFolder").Value
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

        //void SaveTemplates()
        //{
        //    try
        //    {
        //        _invoiceImportTemplates.Save(InvoiceImportTemplatePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.LogThis($"Error saving xml invoice templates: {ex}", eloglevel.error);
        //        throw;
        //    }
        //}

        public TransformResultInfo DoTransform()
        {
            AztecBusinessService.CreateUsfFieldDefIfRequired();

            if (_selectedTemplate != null)
                return Transformer.DoTransform(new List<XElement> {_selectedTemplate});

            return new TransformResultInfo
                       {
                           NumberOfFileErrors = 0,
                           NumberOfFilesProcessed = 0,
                           NumberOfInvoiceLinesProcessed = 0,
                           NumberOfInvoicesProcessed = 0,
                           Message = "No supplier selected"
                       };
        }

        public void UpdateInvoiceImportFieldDefinitions(XElement invoiceTemplate)
        {
            AztecBusinessService.UpdateInvoiceImportFieldDefinitions(invoiceTemplate);
        }

        public void IncreaseAlliantTablesImportRefField()
        {
            AztecBusinessService.IncreaseAlliantTablesImportRefField();
        }

        //public void UpdateAztecPurchaseWithSupplierName(XElement invoiceTemplate)
        //{
        //    AztecBusinessService.UpdateAztecPurchaseWithSupplierName(invoiceTemplate);
        //}

        public void UpdatePurSysVarImportFolder()
        {
            string outputPathToUse;

            if (SelectedTemplate.Attribute("OutputFolder") != null && !string.IsNullOrEmpty(SelectedTemplate.Attribute("OutputFolder").Value))
                outputPathToUse = SelectedTemplate.Attribute("OutputFolder").Value;
            else
                outputPathToUse = InvoiceImportTemplates.Root.Element("ImportSettings").Element("ImportAppliction").Attribute("InvoiceFileLocation").Value;

            AztecBusinessService.UpdatePurSysVarImportFolder(outputPathToUse);
        }

        internal void Save()
        {
            _invoiceImportTemplates.Root.Element("ImportSettings").Element("ImportAppliction").SetAttributeValue("FileName", _importAppLocation);
            _invoiceImportTemplates.Root.Element("ImportSettings").Element("ImportAppliction").SetAttributeValue("InvoiceFileLocation", _importAppInvoiceFileLocation);

            _invoiceImportTemplates.Save(InvoiceImportTemplatePath);
            IsDirty = false;
        }

        public bool IsDirty { get; private set; }
    }
}
