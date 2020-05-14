using System;
using System.Text.RegularExpressions;
using System.IO;

namespace _1C_ZabbixMonitoringApp
{
    public class LogFilesWalker
    {
        private FileInfo[] logFiles;
        private int nextFileIndex = 0;

        private string logFilesPattern = "";

        public LogFilesWalker(string lp)
        {
            DirectoryInfo dir = new DirectoryInfo(@"" + lp);
            logFiles = dir.GetFiles("*.log", SearchOption.AllDirectories);

            DateTime parseDate = DateTime.Now.AddHours(-1);
            logFilesPattern = @"rphost_\d+\\" + parseDate.ToString("yyMMddhh") + @"\.log";
        }

        public string GetNextFilePath()
        {
            string nextFilePath = String.Empty;

            while (nextFileIndex < logFiles.Length)
            {
                nextFileIndex++;

                Match m = Regex.Match(logFiles[nextFileIndex - 1].FullName, logFilesPattern);

                if (m.Success)
                {
                    nextFilePath = logFiles[nextFileIndex - 1].FullName;
                    break;
                }
            }
            
            return nextFilePath;
        }

        private string FormatDateValue(int val)
        {
            return val < 10 ? "0" + val.ToString() : val.ToString();
        }
        
    }
}
