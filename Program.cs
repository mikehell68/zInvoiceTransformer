using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LogThis;

namespace zInvoiceTransformer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            bool result;
            var mutex = new System.Threading.Mutex(true, "ZinvoiceTransformer", out result);

            if (!result)
            {
                MessageBox.Show("Zonal Invoice Import is already running.", "Zonal Invoice Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            InitialiseLogging();
            string appname = Path.GetFileName(Application.ExecutablePath);
            Log.LogThis(string.Format("{0} Started", appname), eloglevel.info);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InvoiceImportMain());
            
            Log.LogThis(string.Format("{0} Ended", appname), eloglevel.info);

            GC.KeepAlive(mutex);
        }

        static void InitialiseLogging()
        {
            const string logLocation = @".\logs";
            if (!Directory.Exists(logLocation))
                Directory.CreateDirectory(logLocation);

            List<string> logFiles = Directory.GetFiles(logLocation).ToList();

            foreach (var logFile in logFiles)
            {
                if (File.GetLastWriteTimeUtc(logFile) < DateTime.Now.AddDays(-28))
                    File.Delete(logFile);
            }

            Log.UseSensibleDefaults(Path.GetFileName(Application.ExecutablePath), logLocation, eloglevel.info);
            Log.LogPrefix = elogprefix.dt_loglevel;
            Log.LogPeriod = elogperiod.day;
            Log.SetLogPath();

            Log.LogThis("Logging initialised", eloglevel.info);
        }
    }
}
