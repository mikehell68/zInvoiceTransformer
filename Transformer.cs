using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LogThis;
using ZinvoiceTransformer.XmlHelpers;
using ZinvoiceTransformer.XmlModels;

namespace zInvoiceTransformer
{
    public enum FieldRecordLocation
    {
        MasterRow = 1,
        DetailFields = 2,
        SummaryRow = 3,
    }

    public class TransformResultInfo
    {
        protected class InvoiceFileInfo
        {
            public class InvoiceStats
            {
                public string InvoiceNumber { get; set; }
                public int TotalInvoiceLines { get; set; }
                public int InvoiceLinesTransformed { get; set; }
                public List<string> NegativeValueLines { get; set; }
            }

            public string FileName { get; set; }
            public List<InvoiceStats> Invoices { get; set; }
           
        }
        
        public int NumberOfFilesProcessed { get; set; }
        public int NumberOfInvoicesProcessed { get; set; }
        public int NumberOfFileErrors { get; set; }
        public int NumberOfInvoiceLinesProcessed { get; set; }
        public string Message { get; set; }
        //List<InvoiceFileInfo> TransformStats; 
    }
    
    static class Transformer
    {
        static XDocument _invoiceImportTemplates;
        static InvoiceImportTemplates _importTemplates;
        public static InvoiceImportTemplates InvoiceImportTemplates
        {
            get => _importTemplates;
            set => _importTemplates = value;
        }

        static InvoiceImportTemplatesTemplateTemplateTransform _templateTransformFields;
        static InvoiceImportTemplatesTemplate _selectedTemplate;

        static string _invoiceNumber = "";
        static string _invoiceDate = "";

        static int _runningStartPos;

        static readonly Dictionary<int, int> _newFieldPositions = new Dictionary<int, int>(); //<fieldNameId, FieldPositionInNewInvoiceFileLine>
        private static bool _useEachesConversion;
        private static string _eachesConverionTag;

        static Transformer()
        {
            LoadTemplates();
        }

        public static TransformResultInfo DoTransform()
        {
            return DoTransform(GetTemplatesInUse());
        }

        public static TransformResultInfo DoTransform(List<int> templateIds)
        {
            var templatesToTransform = _importTemplates.Templates.Where(t => templateIds.Contains(t.Id)).ToList();

            return DoTransform(templatesToTransform);
        }

        public static TransformResultInfo DoTransform(List<InvoiceImportTemplatesTemplate> templates)
        {
            var transformResultInfo = new TransformResultInfo();
            
            foreach (var template in templates)
            {
                transformResultInfo.NumberOfInvoiceLinesProcessed += DoTransform(template).NumberOfInvoiceLinesProcessed;
            }

            SaveTemplates();
            return transformResultInfo;
        }

        private static TransformResultInfo DoTransform(InvoiceImportTemplatesTemplate template)
        {
            Log.LogThis("Starting invoice transform", eloglevel.info);
            Log.LogThis($"Using invoice template: Id: {template.Id} Name: {template.Name}", eloglevel.info);
            Log.LogThis($"Loading original invoice file(s) from: {template.SourceFolder}", eloglevel.info);

            var filelist = Directory.GetFiles(template.SourceFolder).ToList();
            Log.LogThis($"Invocie files found: {string.Join(",", filelist.Select(Path.GetFileName).ToArray())}", eloglevel.info);

            _templateTransformFields = template.TemplateTransform;
            _selectedTemplate = template;
            _selectedTemplate.InvoiceNumbersToUpdate = new List<string>().ToArray();

            var eachesConversionElement = _selectedTemplate.EachesConversion;

            _useEachesConversion = eachesConversionElement != null && eachesConversionElement.enabled == 1;
            _eachesConverionTag = _useEachesConversion && eachesConversionElement?.tag != null ? eachesConversionElement.tag : "";

            var transformResultInfo = new TransformResultInfo();
            var newSplitLine = new ArrayList();

            var fileInvoices = new Dictionary<string, List<string[]>>();

            foreach (string file in filelist)
            {
                Log.LogThis($"Processing file: {file}", eloglevel.info);
                fileInvoices.Add(file, new List<string[]>());

                transformResultInfo.NumberOfFilesProcessed += 1;

                int newFieldPos = 0;
                InvoiceImportTemplatesTemplateMasterRowField invoiceNoTemplateField = null;
                InvoiceImportTemplatesTemplateMasterRowField invoiceDateTemplateField = null;

                var sr = new StreamReader(file);

                if (_selectedTemplate.HasHeaderRecord == 1)
                {
                    sr.ReadLine(); //advance past the header line
                }

                while (!sr.EndOfStream)
                {
                    var csvParser = new CsvParser(_selectedTemplate.Delimiter[0], '\0');

                    var splitOriginalLine = (string[])csvParser.CSVParser(sr.ReadLine()).ToArray(typeof(string));

                    if (splitOriginalLine.Count() == 0)
                        continue;

                    newSplitLine.Clear();

                    if (_selectedTemplate.HasMasterRecord == 1 && IsMasterRow(splitOriginalLine))
                    {
                        invoiceNoTemplateField = _selectedTemplate.MasterRow.Field.FirstOrDefault(x => x.FieldNameId == 1);
                        _invoiceNumber = splitOriginalLine[invoiceNoTemplateField.Delimited.Position];

                        invoiceDateTemplateField = _selectedTemplate.MasterRow.Field.FirstOrDefault(x => x.FieldNameId == 2);
                        _invoiceDate = splitOriginalLine[invoiceDateTemplateField.Delimited.Position];
                    }
                    else if (IsDetailRow(splitOriginalLine))
                    {
                        if (_selectedTemplate.HasMasterRecord == 1)
                        {
                            newSplitLine.Add(_invoiceNumber);
                            SetNewFieldPosition(invoiceNoTemplateField.FieldNameId, ref newFieldPos);

                            newSplitLine.Add(_invoiceDate);
                            SetNewFieldPosition(invoiceDateTemplateField.FieldNameId, ref newFieldPos);
                        }

                        foreach (InvoiceImportTemplatesTemplateDetailFieldsField templateField in _selectedTemplate.DetailFields.Field)
                        {
                            string fieldValue = splitOriginalLine[templateField.Delimited.Position];

                            if (templateField.DirectiveId > 0)
                            {
                                Log.LogThis("Alternative processing directive found for fieldNameId " + templateField.FieldNameId + ", directiveId " + templateField.DirectiveId, eloglevel.info);

                                var fieldDirective = _selectedTemplate.Directives.Directive.Id == templateField.DirectiveId ? _selectedTemplate.Directives.Directive : null;
                                var sourceFieldConditionValue = splitOriginalLine[fieldDirective.Condition.ConditionFieldPosition];

                                Log.LogThis("DirectiveId " + fieldDirective.Id + ": Name: " + fieldDirective.Name, eloglevel.info);

                                decimal result = decimal.Parse(fieldValue);

                                if (sourceFieldConditionValue == fieldDirective.Condition.ConditionValue.ToString())
                                {
                                    var operand1 = decimal.Parse(splitOriginalLine[fieldDirective.Calculation.Operand1.sourceFieldPosition]);
                                    var operand2 = decimal.Parse(splitOriginalLine[fieldDirective.Calculation.Operand2.sourceFieldPosition]);
                                    var op = fieldDirective.Calculation.Operator.OpType;

                                    switch (op)
                                    {
                                        case "division":
                                            result = operand1 / (operand2 == 0 ? 1 : operand2);
                                            Log.LogThis("Performing directive: " + operand1 + " / " + (operand2 == 0 ? 1 : operand2) + " = " + result, eloglevel.info);
                                            break;
                                        case "multiply":
                                            result = operand1 * operand2;
                                            break;
                                        case "add":
                                            result = operand1 + operand2;
                                            break;
                                        case "minus":
                                            result = operand1 - operand2;
                                            break;
                                        default:
                                            break;
                                    }

                                    fieldValue = result.ToString("F4");
                                }
                            }

                            newSplitLine.Add(fieldValue);
                            SetNewFieldPosition(templateField.FieldNameId, ref newFieldPos);
                        }

                        //if (!IgnoreNegativeAmountLines(new[] { "UnitCost", "TotalCost" }, newSplitLine))
                        fileInvoices[file].Add(newSplitLine.OfType<string>().ToArray()); //build a list of all the detail lines
                    }
                    else if (IsSummaryRow(splitOriginalLine))
                    {
                        //ignore summary lines
                    }
                }
                sr.Close();

                transformResultInfo.NumberOfInvoiceLinesProcessed += fileInvoices[file].Count;
                ArchiveProcessedInvoiceFile(file);
            }

            var allDetailLines = fileInvoices.Values.SelectMany(l => l).ToList();
            if (allDetailLines.Count > 0)
            {
                List<string> invoiceNumbersToUpdate = new List<string>();
                foreach (var inv in allDetailLines.Select(invLine => invLine[0]).Distinct().ToList())
                {
                    invoiceNumbersToUpdate.Add(inv);
                    //_selectedTemplate.InvoiceNumbersToUpdate.Add(new XElement("InvoiceNumber", inv));
                }

                _selectedTemplate.InvoiceNumbersToUpdate = invoiceNumbersToUpdate.ToArray();

                CreateFixedLengthFieldsAndTransformDetials(fileInvoices);

                SaveTransformedInvoiceFile(fileInvoices);
            }

            return transformResultInfo;
        }

        private static bool IsSummaryRow(string[] splitLine)
        {
            if (_selectedTemplate.SummaryRow != null)
            {
                if (_selectedTemplate.SummaryRow.Field.Count() == 0)
                    return false;

                int recordTypePostion = _selectedTemplate.SummaryRow.RecordTypePostion;
                var recordTypeIdentifier = _selectedTemplate.SummaryRow.RecordTypeIdentifier;

                return splitLine[recordTypePostion] == recordTypeIdentifier;
            }
            return false;
        }

        private static void SaveTransformedInvoiceFile(Dictionary<string, List<string[]>> allFileInvoices)
        {
            foreach (var fileInvoice in allFileInvoices)
            {
                var file = fileInvoice.Key;
                var filename = Path.GetFileName(file) ?? DateTime.Now.ToLongDateString() + "INV";

                string originalOutputFilePath;

                if (_selectedTemplate.OutputFolder != null && !string.IsNullOrEmpty(_selectedTemplate.OutputFolder))
                    originalOutputFilePath = Path.Combine(_selectedTemplate.OutputFolder, filename);
                else
                    originalOutputFilePath = _importTemplates.ImportSettings.ImportAppliction.InvoiceFileLocation;

                Log.LogThis($"Saving transformed invoice file: {originalOutputFilePath}", eloglevel.info);

                int duplicateFileNameTag = 0;
                var outputFile = originalOutputFilePath;
                while (File.Exists(outputFile))
                {
                    outputFile = originalOutputFilePath + "_" + ++duplicateFileNameTag;
                }

                File.WriteAllText(outputFile, FlattenInvoiceLines(fileInvoice.Value));
            }
        }

        private static void ArchiveProcessedInvoiceFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            var archiveFilePath = Path.Combine(Path.GetDirectoryName(file), "transformed");

            Log.LogThis($"Archiving transformed invoice file: {Path.GetFileName(file)} to {archiveFilePath}", eloglevel.info);
            
            Directory.CreateDirectory(archiveFilePath);

            int duplicateFileNameTag = 0;
            var outputFile = Path.Combine(archiveFilePath, Path.GetFileName(file));
            var outputFileTmp = outputFile;
            while (File.Exists(outputFileTmp))
            {
                outputFileTmp = outputFile + "_" + ++duplicateFileNameTag;
            }

            File.Move(file, outputFileTmp);
        }

        public static List<InvoiceImportTemplatesTemplate> GetTemplatesInUse()
        {
            LoadTemplates();
            var templateIdsToUse = _importTemplates.ImportSettings.ImportOrder.Select(x => x.Id).ToList();
            var templatesToUse = _importTemplates.Templates.Where(x => templateIdsToUse.Contains(x.Id)).ToList();
            return templatesToUse;
        }

        public static void LoadTemplates()
        {
            Log.LogThis("Loading invoice templates", eloglevel.info);
            string templatesXml;
            try
            {
                templatesXml = File.ReadAllText(InvoiceTemplateModel.InvoiceImportTemplatePath);
            }
            catch (Exception ex)
            {
                Log.LogThis($"Error reading invoice templates file: {ex}", eloglevel.error);
                throw;
            }

            //var templatesStringReader = new StringReader(templatesXml);
            try
            {
                _importTemplates = templatesXml.ParseXml<InvoiceImportTemplates>();
                //_invoiceImportTemplates = XDocument.Load(templatesStringReader);
            }
            catch (Exception ex)
            {
                Log.LogThis($"Error loading xml from invoice templates file: {ex}", eloglevel.error);
                throw;
            }
            finally
            {
                //templatesStringReader.Close();
            }
            Log.LogThis($"{_importTemplates.Templates.Count()} invoice templates loaded ", eloglevel.info);
        }

        static void SaveTemplates()
        {
            try
            {
                _invoiceImportTemplates.Save(InvoiceTemplateModel.InvoiceImportTemplatePath);
            }
            catch (Exception ex)
            {
                Log.LogThis(string.Format("Error saving xml invoice templates: {0}", ex), eloglevel.error);
                throw;
            }
        }

        static string FlattenInvoiceLines(List<string[]> invoiceLines)
        {
            List<string> flattenedInvoiceLines = invoiceLines.Select(invLine => string.Join("", invLine)).ToList();

            var fixedWidthInvoiceStrings = new StringBuilder();
            
            foreach (string str in flattenedInvoiceLines)
            {
                fixedWidthInvoiceStrings.Append(str);
                fixedWidthInvoiceStrings.AppendLine();
            }

            return fixedWidthInvoiceStrings.ToString();
        }

        private static void SetNewFieldPosition(byte invoiceFieldId, ref int _newFieldPos)
        {
            int fieldNameId = invoiceFieldId;
            if (!_newFieldPositions.ContainsKey(fieldNameId))
            {
                _newFieldPositions.Add(fieldNameId, _newFieldPos);
                _newFieldPos += 1;
            }
        }

        private static void CreateFixedLengthFieldsAndTransformDetials(Dictionary<string, List<string[]>> fileInvoiceDetailLines)
        {
            Log.LogThis("Creating fixed width invoice lines", eloglevel.info);
            _runningStartPos = 1;
            foreach (KeyValuePair<int, int> newFieldPosition in _newFieldPositions.OrderBy(x => x.Key))
            {
                KeyValuePair<int, int> fieldPosition = newFieldPosition; //this is the position of a field in the newly created invoice line

                var fieldDataType = _templateTransformFields.Fields.
                                        Where(field => (int)field.FieldNameId == fieldPosition.Key).
                                        FirstOrDefault().DataType;

                var allInvoiceLines = fileInvoiceDetailLines.Values.SelectMany(l => l).ToList();
                int maxFieldlength = allInvoiceLines.Max(x => x[fieldPosition.Value].Length); //get the max length of the field from all fields at a position
                
                foreach (KeyValuePair<string, List<string[]>> invoiceDetailLine in fileInvoiceDetailLines)
                {
                    switch (fieldDataType)
                    {
                        case "string":
                            if (fieldPosition.Key == 6) //USFoodsImport expects UOM is 2 chars: fieldNameId = 6
                            {
                                maxFieldlength = 2;
                                //use 'CS' as the default for empty UOM values
                                invoiceDetailLine.Value.ForEach(invLine => invLine[fieldPosition.Value] = string.IsNullOrWhiteSpace(invLine[fieldPosition.Value]) || invLine[fieldPosition.Value] == "CA" ? invLine[fieldPosition.Value] = "CS  " : invLine[fieldPosition.Value]);
                            }
                            else if (fieldPosition.Key == 9 && _useEachesConversion)
                            {
                                foreach (var invLine in invoiceDetailLine.Value)
                                {
                                    if (invLine[5] == "EA")
                                        invLine[fieldPosition.Value] = invLine[fieldPosition.Value].Trim(' ') + _eachesConverionTag;
                                }
                                maxFieldlength += _eachesConverionTag.Length;
                            }

                            invoiceDetailLine.Value.ForEach(invLine => invLine[fieldPosition.Value] = invLine[fieldPosition.Value].Trim(' ').PadRight(maxFieldlength, ' ')); //pad out all the fields in that position to the max field length
                            break;

                        case "date":
                            var dateFormat = _templateTransformFields.Fields.
                                                    Where(field => (int)field.FieldNameId == fieldPosition.Key).
                                                    FirstOrDefault().DateFormat;

                            invoiceDetailLine.Value.ForEach(invLine => invLine[fieldPosition.Value] = DateTime.ParseExact(invLine[fieldPosition.Value], dateFormat, CultureInfo.InvariantCulture).ToString("MMddyy"));
                            maxFieldlength = 6;
                            break;

                        case "int":
                            if (fieldPosition.Key == 7 || fieldPosition.Key == 8)
                            {
                                invoiceDetailLine.Value.ForEach(invLine => FormatNumericField(invLine, fieldPosition, maxFieldlength));
                            }
                            else
                            {
                                invoiceDetailLine.Value.ForEach(invLine => FormatNumericField(invLine, fieldPosition, maxFieldlength, false));
                            }
                            break;
                    }
                    
                }
                SetInvoiceFieldTransformData(ref _runningStartPos, fieldPosition.Key, maxFieldlength); // set the transfrom values for the field to be used by USFood Import
            }
        }

        private static string FormatNumericField(string[] invoiceLine, KeyValuePair<int, int> fieldPosition, int maxFieldlength, bool isMoney = true)
        {
            string fieldValue = invoiceLine[fieldPosition.Value];
            fieldValue = fieldValue.Trim(' ');

            if(isMoney)
                fieldValue = (Decimal.Parse(fieldValue) * 100).ToString("F0");

            if (!isMoney && fieldValue.Contains("."))
                fieldValue = Decimal.Parse(fieldValue).ToString();

            fieldValue = fieldValue.PadLeft(maxFieldlength, '0');
            
            if (fieldValue.Contains("-"))
            {
                fieldValue = fieldValue.Remove(fieldValue.IndexOf('-'), 1);
                fieldValue = "-" + fieldValue;
            }
             
            return invoiceLine[fieldPosition.Value] = fieldValue;
        }

        private static void SetInvoiceFieldTransformData(ref int runningStartPos, int fieldNameId, int maxFieldlength)
        {
            var y = _templateTransformFields.Fields.Where(x => (int)x.FieldNameId == fieldNameId).FirstOrDefault();
            y.Start = (byte)runningStartPos;
            y.Length = (byte)maxFieldlength;
            runningStartPos += maxFieldlength;
        }

        private static bool IsDetailRow(string[] splitLine)
        {
            if (_selectedTemplate.DetailFields.Field.Count() > 0)
            {
                int recordTypePostion = _selectedTemplate.DetailFields.RecordTypePostion;
                var recordTypeIdentifier = _selectedTemplate.DetailFields.RecordTypeIdentifier;

                if (recordTypePostion == -1)
                    return true;

                return splitLine[recordTypePostion] == recordTypeIdentifier;
            }
            return false;
        }

        private static bool IsMasterRow(string[] splitLine)
        {
            if (_selectedTemplate.MasterRow.Field.Count() > 0)
            {
                if (_selectedTemplate.MasterRow == null)
                    return false;

                int recordTypePostion = _selectedTemplate.MasterRow.RecordTypePostion;
                var recordTypeIdentifier = _selectedTemplate.MasterRow.RecordTypeIdentifier;

                return splitLine[recordTypePostion] == recordTypeIdentifier;
            }
            return false;
        }
    }
}
