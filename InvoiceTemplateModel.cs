using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LogThis;
using System.ComponentModel;
using ZinvoiceTransformer.XmlHelpers;
using ZinvoiceTransformer.XmlModels;

namespace zInvoiceTransformer
{
    public class InvoiceTemplateModel : INotifyPropertyChanged
    {
        public const string InvoiceImportTemplatePath = @"InvoiceImportTemplates.xml";
        public event PropertyChangedEventHandler PropertyChanged;

        public InvoiceImportTemplates ImportTemplates { get; private set; }

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

        InvoiceImportTemplatesTemplate _selectedTemplate;
        public InvoiceImportTemplatesTemplate SelectedTemplate
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
            _importAppLocation = ImportTemplates.ImportSettings.ImportAppliction.FileName;
            //_importAppLocation = _invoiceImportTemplates.Root.Element("ImportSettings").Element("ImportAppliction").Attribute("FileName").Value;
            _importAppInvoiceFileLocation = ImportTemplates.ImportSettings.ImportAppliction.InvoiceFileLocation;
            //_importAppInvoiceFileLocation = _invoiceImportTemplates.Root.Element("ImportSettings").Element("ImportAppliction").Attribute("InvoiceFileLocation").Value;
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

            //var templatesStringReader = new StringReader(templatesXml);
            try
            {
                ImportTemplates = templatesXml.ParseXml<InvoiceImportTemplates>();
                //_invoiceImportTemplates = XDocument.Load(templatesStringReader);
            }
            catch (Exception ex)
            {
                Log.LogThis($"Error loading xml from invoice templates file: {ex}", eloglevel.error);
                throw;
            }
            //finally
            //{
            //    templatesStringReader.Close();
            //}
            Log.LogThis($"{ImportTemplates.Templates.Count()} invoice templates loaded ", eloglevel.info);
            
            //Log.LogThis(
            //    $"{_invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Count()} invoice templates loaded ", eloglevel.info);
        }

        public TemplateListItem[] GetAllTemplatesArray()
        {
            return ImportTemplates.Templates.Select(
                template => new TemplateListItem
                {
                    Id = template.Id.ToString(),
                    Name = template.Name,
                    IsInUse = template.Active == 1
                }).ToArray();

            //if (_invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Count() > 0)
            //{
            //    return _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Select(
            //        template => new TemplateListItem
            //                        {
            //                            Id = template.Attribute("Id").Value,
            //                            Name = template.Attribute("Name").Value,
            //                            IsInUse = template.Attribute("Active").Value == "1"
            //                        }).ToArray();
            //}

            //return new TemplateListItem[0];
        }

        //public IEnumerable<XElement> GetAllTemplates()
        //{
        //    if (_invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Count() > 0)
        //    {
        //        return _invoiceImportTemplates.Root.Element("Templates").Descendants("Template");
        //    }

        //    return null;
        //}

        //public IEnumerable<XElement> GetAllActiveTemplates()
        //{
        //    var allActiveTemplates = GetAllTemplates();
            
        //    if (allActiveTemplates != null)
        //    {
        //        return GetAllTemplates().Where(t => t.Attribute("Active").Value == "1");
        //    }

        //    return new List<XElement>();
        //}

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

        public void SetSelectedTemplate(byte selectedTemplateId)
        {
            var selectedTemplate = ImportTemplates.Templates.FirstOrDefault(t => t.Id == selectedTemplateId);
            SelectedTemplateName = selectedTemplate == null ? "" : selectedTemplate.Name;
            SelectedTemplateDescription = selectedTemplate == null ? "" : selectedTemplate.Description;
            IsDirty = false;
        }

        public string[] GetSelectedTemplateImportFiles()
        {
            if (SelectedTemplate == null)
                return new string[]{ "<-no supplier selected->" };

            string sourceFolder = SelectedTemplate.SourceFolder;
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
                        DestinationFolder = SelectedTemplate.SourceFolder,
                        InvoiceFilePrefix = SelectedTemplate.RemoteInvoiceSettings.InvoiceFileCustomerPrefix,
                        HostUrl = SelectedTemplate.RemoteInvoiceSettings.url,
                        Port = SelectedTemplate.RemoteInvoiceSettings.port,
                        Username = SelectedTemplate.RemoteInvoiceSettings.username,
                        Password = SelectedTemplate.RemoteInvoiceSettings.password,
                        RemoteFolder = SelectedTemplate.RemoteInvoiceSettings.RemoteFolder
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
                return Transformer.DoTransform(new List<InvoiceImportTemplatesTemplate> {_selectedTemplate});

            return new TransformResultInfo
                       {
                           NumberOfFileErrors = 0,
                           NumberOfFilesProcessed = 0,
                           NumberOfInvoiceLinesProcessed = 0,
                           NumberOfInvoicesProcessed = 0,
                           Message = "No supplier selected"
                       };
        }

        public void UpdateInvoiceImportFieldDefinitions(InvoiceImportTemplatesTemplate invoiceTemplate)
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

            if (SelectedTemplate.OutputFolder != null && !string.IsNullOrEmpty(SelectedTemplate.OutputFolder))
                outputPathToUse = SelectedTemplate.OutputFolder;
            else
                outputPathToUse = ImportTemplates.ImportSettings.ImportAppliction.InvoiceFileLocation;

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
