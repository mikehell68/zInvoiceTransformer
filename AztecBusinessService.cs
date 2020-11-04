using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using LogThis;

namespace ZinvoiceTransformer
{
    internal static class AztecBusinessService
    {
        private const string _databaseMachine = "(local)";
        private const string _databaseName = "Aztec";

#if DEBUG 
        private const string dbConnectionString = "Trusted_Connection=True;Initial Catalog={0};Data Source={1}";
#elif !DEBUG
        private const string dbConnectionString = "User ID=zonalsysadmin;Password=0049356GNHsxkzi26TYMF;Initial Catalog={0};Data Source={1}";
#endif

        private static XDocument _sqlScripts;

        static AztecBusinessService()
        {
            LoadSqlScripts();
        }

        private static string GetFromResources(string resourceName)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            using (Stream stream = assem.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        internal static void LoadSqlScripts()
        {
            Log.LogThis("Loading scripts", eloglevel.info);
            string sqlScriptsXml;
            try
            {
                sqlScriptsXml = GetFromResources("ZinvoiceTransformer.Scripts.SqlScripts.xml");
            }
            catch (Exception ex)
            {
                Log.LogThis(string.Format("Error reading scripts file: {0}", ex), eloglevel.error);
                throw;
            }

            var sqlScriptsStringReader = new StringReader(sqlScriptsXml);
            try
            {
                _sqlScripts = XDocument.Load(sqlScriptsStringReader);
            }
            catch (Exception ex)
            {
                Log.LogThis(string.Format("Error loading scripts: {0}", ex), eloglevel.error);
                throw;
            }
            finally
            {
                sqlScriptsStringReader.Close();
            }
            Log.LogThis(string.Format("{0} scripts loaded ", _sqlScripts.Root.Descendants("SqlScript").Count()), eloglevel.info);
        }
        
        internal static void UpdateInvoiceImportFieldDefinitions(XElement invoiceTemplate)
        {
            using (var sqlConnection = new SqlConnection(String.Format(dbConnectionString, _databaseName, _databaseMachine)))
            {
                sqlConnection.Open();
                var cmd = sqlConnection.CreateCommand();
                var sqlStrings = new StringBuilder();
                var sqlScript = _sqlScripts.Descendants("SqlScript").Where(
                        s => s.Attribute("Name").Value == "UpdateFieldDefinitions").FirstOrDefault().Value;

                foreach (var field in invoiceTemplate.Element("TemplateTransform").Element("Fields").Descendants("Field"))
                {
                    sqlStrings.AppendFormat(sqlScript, field.Attribute("Start").Value, field.Attribute("Length").Value, field.Attribute("FieldNameId").Value);
                    sqlStrings.AppendLine();
                }

                cmd.CommandText = sqlStrings.ToString();
                cmd.ExecuteNonQuery();

                sqlScript = _sqlScripts.Descendants("SqlScript").Where(
                        s => s.Attribute("Name").Value == "UpdateLbProcessingFieldDefinition").FirstOrDefault().Value;
                
                cmd.CommandText = string.Format(sqlScript, invoiceTemplate.Attribute("LbProcessingType").Value);
                cmd.ExecuteNonQuery();

                sqlScript = _sqlScripts.Descendants("SqlScript").Where(
                        s => s.Attribute("Name").Value == "UpdateSupplierNameFieldDefinition").FirstOrDefault().Value;

                cmd.CommandText = string.Format(sqlScript, invoiceTemplate.Attribute("Name").Value);
                cmd.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }

        internal static void UpdatePurSysVarImportFolder(string outputPath)
        {
            using (var sqlConnection = new SqlConnection(String.Format(dbConnectionString, _databaseName, _databaseMachine)))
            {
                sqlConnection.Open();
                var cmd = sqlConnection.CreateCommand();
                var sqlScript = _sqlScripts.Descendants("SqlScript").Where(
                        s => s.Attribute("Name").Value == "UpdatePurSysVarImportFolder").FirstOrDefault().Value;

                cmd.CommandText = string.Format(sqlScript, outputPath);
                cmd.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }

        internal static void IncreaseAlliantTablesImportRefField()
        {
            using (var sqlConnection = new SqlConnection(String.Format(dbConnectionString, _databaseName, _databaseMachine)))
            {
                sqlConnection.Open();
                var cmd = sqlConnection.CreateCommand();
                var sqlScript = _sqlScripts.Descendants("SqlScript").Where(
                        s => s.Attribute("Name").Value == "UpdateAlliantImportRefFieldSize").FirstOrDefault().Value;

                cmd.CommandText = sqlScript;
                cmd.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }

        //internal static void UpdateAztecPurchaseWithSupplierName(XElement invoiceTemplate)
        //{
        //    string sqlInString = "(" + string.Join(",", invoiceTemplate.Element("InvoiceNumbersToUpdate").Descendants().Select(x => "'" + x.Value + "'")) + ")";

        //    string sqlUpdateString = _sqlScripts.Descendants("SqlScript").Where(
        //                s => s.Attribute("Name").Value == "UpdatePurchHdrAztecSupplier").FirstOrDefault().Value;

        //    string sqlUpdatePurchaseString = _sqlScripts.Descendants("SqlScript").Where(
        //                s => s.Attribute("Name").Value == "UpdatePurchaseAztecSupplier").FirstOrDefault().Value;

        //    string sqlUpdatePurchReferenceString = _sqlScripts.Descendants("SqlScript").Where(
        //                s => s.Attribute("Name").Value == "UpdatePurchReferenceSupplier").FirstOrDefault().Value;

        //    using (var sqlConnection = new SqlConnection(String.Format(dbConnectionString, _databaseName, _databaseMachine)))
        //    {
        //        sqlConnection.Open();
        //        var cmd = sqlConnection.CreateCommand();

        //        cmd.CommandText = string.Format(sqlUpdateString, invoiceTemplate.Attribute("Name").Value, sqlInString);
        //        var recsUpdated = cmd.ExecuteNonQuery();
        //        Log.LogThis(string.Format("{0} records updated in PurchHdrAztec", recsUpdated), eloglevel.info);

        //        cmd.CommandText = sqlUpdatePurchaseString;
        //        recsUpdated = cmd.ExecuteNonQuery();
        //        Log.LogThis(string.Format("{0} records updated in Purchase", recsUpdated), eloglevel.info);

        //        cmd.CommandText = sqlUpdatePurchReferenceString;
        //        recsUpdated = cmd.ExecuteNonQuery();
        //        Log.LogThis(string.Format("{0} records updated in PurchReference", recsUpdated), eloglevel.info);

        //        sqlConnection.Close();
        //    }
        //}

        internal static void CreateUsfFieldDefIfRequired()
        {
            Log.LogThis("Loading usfFieldDef creation script", eloglevel.info);
            string sqlScript;
            try
            {
                sqlScript = GetFromResources("ZinvoiceTransformer.Scripts.USFoodsScript.sql");
            }
            catch (Exception ex)
            {
                Log.LogThis(string.Format("Error reading script file: {0}", ex), eloglevel.error);
                throw;
            }
            
            using (var sqlConnection = new SqlConnection(String.Format(dbConnectionString, _databaseName, _databaseMachine)))
            {
                sqlConnection.Open();
                var cmd = sqlConnection.CreateCommand();
                
                cmd.CommandText = sqlScript;
                var recsUpdated = cmd.ExecuteNonQuery();
                Log.LogThis(string.Format("{0} records updated", recsUpdated), eloglevel.info);
                sqlConnection.Close();
            }
        }
    }
}
