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
        
        //XDocument _invoiceImportTemplates;
        //public XDocument InvoiceImportTemplates
        //{
        //    get => _invoiceImportTemplates;
        //    set => _invoiceImportTemplates = value;
        //}

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
            CreateWorkingFolders();
            _importAppLocation = ImportTemplates.ImportSettings.ImportAppliction.FileName;
            _importAppInvoiceFileLocation = ImportTemplates.ImportSettings.ImportAppliction.InvoiceFileLocation;
        }

        private void CreateWorkingFolders()
        {
            Log.LogThis("Creating working folders", eloglevel.info);

            if (ImportTemplates?.Folders == null || ImportTemplates.Folders.ToList().Count == 0)
            {
                Log.LogThis("No working folders defined", eloglevel.info);
                return;
            }

            foreach (var folder in ImportTemplates.Folders)
            {
                var path = folder.Path ?? "";
                try
                {
                    if (!Directory.Exists(path) && folder.AutoCreate)
                        Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.LogThis($"Unable to create folder '{path}'", eloglevel.error);
                    Log.LogThis($"{ex}", eloglevel.error);
                }
            }
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

            try
            {
                ImportTemplates = templatesXml.ParseXml<InvoiceImportTemplates>();
            }
            catch (Exception ex)
            {
                Log.LogThis($"Error loading xml from invoice templates file: {ex}", eloglevel.error);
                throw;
            }
            
            Log.LogThis($"{ImportTemplates.Templates.Count()} invoice templates loaded ", eloglevel.info);
        }

        public TemplateListItem[] GetAllTemplatesArray()
        {
            return ImportTemplates.Templates.Where(t => t.Active).Select(
                template => new TemplateListItem
                {
                    Id = template.Id.ToString(),
                    Name = template.Name,
                    IsInUse = template.Active
                }).ToArray();
        }

        public InvoiceImportTemplatesTemplate GetTemplate(int templateId)
        {
            InvoiceImportTemplatesTemplate template = null;

            if (ImportTemplates.Templates.Any())
            {
                template = ImportTemplates.Templates.FirstOrDefault(t => t.Id == Convert.ToByte(templateId));
            }

            SelectedTemplateName = template == null ? "" : template.Name;
            SelectedTemplateDescription = template == null ? "" : template.Description;
            IsDirty = false;
            return template;
        }

        public void SetSelectedTemplate(byte selectedTemplateId)
        {
            SelectedTemplate = ImportTemplates.Templates.FirstOrDefault(t => t.Id == selectedTemplateId);
            SelectedTemplateName = SelectedTemplate == null ? "" : SelectedTemplate.Name;
            SelectedTemplateDescription = SelectedTemplate == null ? "" : SelectedTemplate.Description;
            IsDirty = false;
        }

        public string[] GetSelectedTemplateImportFiles()
        {
            if (SelectedTemplate == null)
                return new[]{ "<-no supplier selected->" };

            var sourceFolder = SelectedTemplate.SourceFolder;
            var files = new List<string>
            {
                sourceFolder
            };

            if (!Directory.Exists(sourceFolder)) 
                return new[] {sourceFolder, "<-folder not found->"};

            var fileList = Directory.GetFiles(sourceFolder).Select(Path.GetFileName).ToArray();

            if (!fileList.Any()) 
                return new[] {sourceFolder, "<-folder empty->"};
            
            files.AddRange(fileList);
            return files.ToArray();
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
                        RemoteFolder = SelectedTemplate.RemoteInvoiceSettings.RemoteFolder,
                        DeleteRemoteFileAfterDownload = SelectedTemplate.RemoteInvoiceSettings.DeleteRemoteFileAfterDownload
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
            ImportTemplates.ImportSettings.ImportAppliction.FileName = _importAppLocation;
            ImportTemplates.ImportSettings.ImportAppliction.InvoiceFileLocation = _importAppInvoiceFileLocation;

            ImportTemplates.Save<InvoiceImportTemplates>(InvoiceImportTemplatePath);
            
            IsDirty = false;
        }

        public bool IsDirty { get; private set; }
    }
}
