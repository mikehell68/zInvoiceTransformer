using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LogThis;

namespace ZinvoiceTransformer
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
        public static XDocument InvoiceImportTemplates
        {
            get { return _invoiceImportTemplates; }
            set { _invoiceImportTemplates = value; }
        }

        static XElement _templateTransformFields;
        static XElement _selectedTemplate;

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
            var templatesToTransform = _invoiceImportTemplates.Root.Element("Templates").Descendants("Template").Where(t => templateIds.Contains(int.Parse(t.Attribute("Id").Value))).ToList();

            return DoTransform(templatesToTransform);
        }

        public static TransformResultInfo DoTransform(List<XElement> templates)
        {
            var transformResultInfo = new TransformResultInfo();
            
            foreach (var template in templates)
            {
                transformResultInfo.NumberOfInvoiceLinesProcessed += DoTransform(template).NumberOfInvoiceLinesProcessed;
            }

            SaveTemplates();
            return transformResultInfo;
        }

        private static TransformResultInfo DoTransform(XElement template)
        {
            Log.LogThis("Starting invoice transform", eloglevel.info);
            Log.LogThis(string.Format("Using invoice template: Id: {0} Name: {1}", template.Attribute("Id").Value, template.Attribute("Name").Value), eloglevel.info);
            Log.LogThis(string.Format("Loading original invoice file(s) from: {0}", template.Attribute("SourceFolder").Value), eloglevel.info);
            
            List<string> filelist = Directory.GetFiles(template.Attribute("SourceFolder").Value).OfType<string>().ToList();
            Log.LogThis(string.Format("Invocie files found: {0}", string.Join(",", filelist.Select(Path.GetFileName).ToArray())), eloglevel.info);

            _templateTransformFields = template.Descendants("TemplateTransform").FirstOrDefault();
            _selectedTemplate = template;
            _selectedTemplate.Element("InvoiceNumbersToUpdate").RemoveAll();

            var eachesConversionElement = _selectedTemplate.Element("EachesConversion");

            _useEachesConversion = (eachesConversionElement != null && eachesConversionElement.Attribute("enabled") != null && eachesConversionElement.Attribute("enabled").Value == "1");
            _eachesConverionTag = _useEachesConversion && eachesConversionElement.Attribute("tag") != null ? eachesConversionElement.Attribute("tag").Value : "";
            
            var transformResultInfo = new TransformResultInfo();
            var newSplitLine = new ArrayList();
            
            var fileInvoices = new Dictionary<string, List<string[]>>();
            
            foreach (string file in filelist)
            {
                Log.LogThis(string.Format("Processing file: {0}", file), eloglevel.info);
                fileInvoices.Add(file, new List<string[]>());

                transformResultInfo.NumberOfFilesProcessed += 1;

                int newFieldPos = 0;
                XElement invoiceNoTemplateField = null;
                XElement invoiceDateTemplateField = null;

                var sr = new StreamReader(file);

                if (_selectedTemplate.Attribute("HasHeaderRecord").Value == "1")
                {
                    sr.ReadLine(); //advance past the header line
                }

                while (!sr.EndOfStream)
                {
                    var csvParser = new CsvParser(_selectedTemplate.Attribute("Delimiter").Value[0], '\0');

                    var splitOriginalLine = (string[])csvParser.CSVParser(sr.ReadLine()).ToArray(typeof(string));
                    
                    if (splitOriginalLine.Count() == 0)
                        continue;

                    newSplitLine.Clear();

                    if (_selectedTemplate.Attribute("HasMasterRecord").Value == "1" && IsMasterRow(splitOriginalLine))
                    {
                        invoiceNoTemplateField = _selectedTemplate.Descendants("MasterRow").Descendants("Field").FirstOrDefault(x => x.Attribute("FieldNameId").Value == "1");
                        _invoiceNumber = splitOriginalLine[int.Parse(invoiceNoTemplateField.Descendants("Delimited").First().Attribute("Position").Value)];

                        invoiceDateTemplateField = _selectedTemplate.Descendants("MasterRow").Descendants("Field").FirstOrDefault(x => x.Attribute("FieldNameId").Value == "2");
                        _invoiceDate = splitOriginalLine[int.Parse(invoiceDateTemplateField.Descendants("Delimited").First().Attribute("Position").Value)];
                    }
                    else if (IsDetailRow(splitOriginalLine))
                    {
                        if (_selectedTemplate.Attribute("HasMasterRecord").Value == "1")
                        {
                            newSplitLine.Add(_invoiceNumber);
                            SetNewFieldPosition(invoiceNoTemplateField, ref newFieldPos);

                            newSplitLine.Add(_invoiceDate);
                            SetNewFieldPosition(invoiceDateTemplateField, ref newFieldPos);
                        }

                        foreach (XElement templateField in _selectedTemplate.Element("DetailFields").Descendants("Field"))
                        {
                            string fieldValue = splitOriginalLine[int.Parse(templateField.Descendants("Delimited").First().Attribute("Position").Value)];

                            if (templateField.Attribute("DirectiveId") != null &&
                                !string.IsNullOrEmpty(templateField.Attribute("DirectiveId").Value))
                            {
                                Log.LogThis("Alternative processing directive found for fieldNameId " + templateField.Attribute("FieldNameId").Value + ", directiveId " + templateField.Attribute("DirectiveId").Value, eloglevel.info);
                                
                                var fieldDirective = _selectedTemplate.Element("Directives").Descendants("Directive").FirstOrDefault(directive => directive.Attribute("Id").Value == templateField.Attribute("DirectiveId").Value);
                                var sourceFieldConditionValue = splitOriginalLine[int.Parse(fieldDirective.Element("Condition").Attribute("ConditionFieldPosition").Value)];
                                
                                Log.LogThis("DirectiveId " + fieldDirective.Attribute("Id").Value + ": Name: " + fieldDirective.Attribute("Name").Value, eloglevel.info);

                                decimal result = decimal.Parse(fieldValue);

                                if (sourceFieldConditionValue == fieldDirective.Element("Condition").Attribute("ConditionValue").Value)
                                {
                                    var operand1 = decimal.Parse(splitOriginalLine[int.Parse(fieldDirective.Element("Calculation").Element("Operand1").Attribute("sourceFieldPosition").Value)]);
                                    var operand2 = decimal.Parse(splitOriginalLine[int.Parse(fieldDirective.Element("Calculation").Element("Operand2").Attribute("sourceFieldPosition").Value)]);
                                    var op = fieldDirective.Element("Calculation").Element("Operator").Attribute("OpType").Value;

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
                            SetNewFieldPosition(templateField, ref newFieldPos);
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
                foreach (var inv in allDetailLines.Select(invLine => invLine[0]).Distinct().ToList())
                    _selectedTemplate.Element("InvoiceNumbersToUpdate").Add(new XElement("InvoiceNumber", inv));

                CreateFixedLengthFieldsAndTransformDetials(fileInvoices);

                SaveTransformedInvoiceFile(fileInvoices);
            }

            return transformResultInfo;
        }

        private static bool IsSummaryRow(string[] splitLine)
        {
            if (_selectedTemplate.Element("SummaryRow").Descendants("Field").Count() > 0)
            {
                if (_selectedTemplate.Element("SummaryRow").Attributes().Count() == 0)
                    return false;

                int recordTypePostion = int.Parse(_selectedTemplate.Element("SummaryRow").Attribute("RecordTypePostion").Value);
                var recordTypeIdentifier = _selectedTemplate.Element("SummaryRow").Attribute("RecordTypeIdentifier").Value;

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

                if (_selectedTemplate.Attribute("OutputFolder") != null && !string.IsNullOrEmpty(_selectedTemplate.Attribute("OutputFolder").Value))
                    originalOutputFilePath = Path.Combine(_selectedTemplate.Attribute("OutputFolder").Value, filename);
                else
                    originalOutputFilePath = _invoiceImportTemplates.Root.Element("ImportSettings").Element("ImportAppliction").Attribute("InvoiceFileLocation").Value;

                Log.LogThis(string.Format("Saving transformed invoice file: {0}", originalOutputFilePath), eloglevel.info);

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

            Log.LogThis(string.Format("Archiving transformed invoice file: {0} to {1}", Path.GetFileName(file), archiveFilePath), eloglevel.info);
            
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

        public static List<XElement> GetTemplatesInUse()
        {
            LoadTemplates();
            var templateIdsToUse = _invoiceImportTemplates.Root.Element("ImportSettings").Descendants("ImportOrder").Descendants("Template").Attributes("Id").Select(x => x.Value).ToList();
            var templatesToUse = _invoiceImportTemplates.Root.Descendants("Templates").Descendants("Template").Where(x => templateIdsToUse.Contains(x.Attribute("Id").Value)).ToList();
            return templatesToUse;
        }

        public static void LoadTemplates()
        {
            Log.LogThis("Loading invoice templates", eloglevel.info);
            string templatesXml;
            try
            {
                templatesXml = File.ReadAllText(@"InvoiceImportTemplates.xml");
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

        static void SaveTemplates()
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

        private static void SetNewFieldPosition(XElement invoiceFieldDefinition, ref int _newFieldPos)
        {
            int fieldNameId = int.Parse(invoiceFieldDefinition.Attribute("FieldNameId").Value);
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

                var fieldDataType = _templateTransformFields.Descendants("Fields").Descendants("Field").
                                        Where(field => field.Attribute("FieldNameId").Value == fieldPosition.Key.ToString()).
                                        FirstOrDefault().Attribute("DataType").Value;
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
                            var dateFormat = _templateTransformFields.Descendants("Fields").Descendants("Field").
                                                    Where(field => field.Attribute("FieldNameId").Value == fieldPosition.Key.ToString()).
                                                    FirstOrDefault().Attribute("DateFormat").Value;

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
            XElement y = _templateTransformFields.Descendants("Fields").Descendants("Field").Where(x => x.Attribute("FieldNameId").Value == fieldNameId.ToString()).FirstOrDefault();
            y.SetAttributeValue("Start", runningStartPos);
            y.SetAttributeValue("Length", maxFieldlength);
            runningStartPos += maxFieldlength;
        }

        private static bool IsDetailRow(string[] splitLine)
        {
            if (_selectedTemplate.Element("DetailFields").Descendants("Field").Count() > 0)
            {
                int recordTypePostion = int.Parse(_selectedTemplate.Element("DetailFields").Attribute("RecordTypePostion").Value);
                var recordTypeIdentifier = _selectedTemplate.Element("DetailFields").Attribute("RecordTypeIdentifier").Value;

                if (recordTypePostion == -1)
                    return true;

                return splitLine[recordTypePostion] == recordTypeIdentifier;
            }
            return false;
        }

        private static bool IsMasterRow(string[] splitLine)
        {
            if (_selectedTemplate.Element("MasterRow").Descendants("Field").Count() > 0)
            {
                if (_selectedTemplate.Element("MasterRow").Attributes().Count() == 0)
                    return false;

                int recordTypePostion = int.Parse(_selectedTemplate.Element("MasterRow").Attribute("RecordTypePostion").Value);
                var recordTypeIdentifier = _selectedTemplate.Element("MasterRow").Attribute("RecordTypeIdentifier").Value;

                return splitLine[recordTypePostion] == recordTypeIdentifier;
            }
            return false;
        }
    }
}
