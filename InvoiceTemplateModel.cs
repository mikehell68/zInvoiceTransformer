using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LogThis;
using System.ComponentModel;

namespace ZinvoiceTransformer
{
    public class InvoiceTemplateModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
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
            get { return _invoiceImportTemplates; }
            set { _invoiceImportTemplates = value; }
        }

        XElement _selectedTemplate;
        public XElement SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value; }
        }

        string _selectedTemplateName;
        public string SelectedTemplateName
        {
            get { return _selectedTemplateName; }
            private set { SetField(ref _selectedTemplateName, value, "SelectedTemplateName"); }
        }

        string _selectedTemplateDescription;
        public string SelectedTemplateDescription
        {
            get { return _selectedTemplateDescription; }
            private set { SetField(ref _selectedTemplateDescription, value, "SelectedTemplateDescription"); }
        }

        string _importAppLocation;
        public string ImportAppLocation
        { 
            get { return _importAppLocation; } 
            set { SetField(ref _importAppLocation, value, "ImportAppLocation"); }
        }

        string _importAppInvoiceFileLocation;
        public string ImportAppInvoiceFileLocation
        {
            get { return _importAppInvoiceFileLocation; }
            set { SetField(ref _importAppInvoiceFileLocation, value, "ImportAppInvoiceFileLocation"); }
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
                templatesXml = File.ReadAllText(@"xml\InvoiceImportTemplates.xml");
            }
            catch (Exception ex)
            {
                Log.LogThis(string.Format("Error reading invoice templates file: {0}", ex), eloglevel.error);
                throw;
            }

            var templatesStringReader = new StringReader(templatesXml);
            try
            {
                _invoiceImportTemplates = XDocument.Load(templatesStringReader);
            }
            catch (Exception ex)
            {
                Log.LogThis(string.Format("Error loading xml from invoice templates file: {0}", ex), eloglevel.error);
                throw;
            }
            finally
            {
                templatesStringReader.Close();
            }
            Log.LogThis(string.Format("{0} invoice templates loaded ", _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Count()), eloglevel.info);
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

            return allActiveTemplates;
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
            List<string> files = new List<string>();
            files.Add(sourceFolder);

            if (Directory.Exists(sourceFolder))
            {
                var fileList = Directory.GetFiles(sourceFolder).Select(Path.GetFileName).ToArray();

                if (fileList.Count() > 0)
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
            if (SelectedTemplate != null)
                return new RemoteInvoiceConnectionInfo
                {
                    DestinationFolder = SelectedTemplate.Attribute("SourceFolder").Value,
                    InvoiceFilePrefix = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("InvoiceFileCustomerPrefix").Value,
                    Url = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("url").Value,
                    Port = Convert.ToInt32(SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("port").Value),
                    Username = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("username").Value,
                    Password = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("password").Value,
                    RemoteFolder = SelectedTemplate.Element("RemoteInvoiceSettings").Attribute("RemoteFolder").Value
                };

            return new RemoteInvoiceConnectionInfo();
        }

        void SaveTemplates()
        {
            try
            {
                _invoiceImportTemplates.Save(@"InvoiceImportTemplates.xml");
            }
            catch (Exception ex)
            {
                Log.LogThis(string.Format("Error saving xml invoice templates: {0}", ex), eloglevel.error);
                throw;
            }
        }

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

        public void UpdatePurSysVarImportFolder(XElement invoiceTemplate)
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

            _invoiceImportTemplates.Save(@"InvoiceImportTemplates.xml");
            IsDirty = false;
        }

        public bool IsDirty { get; private set; }
    }
}
