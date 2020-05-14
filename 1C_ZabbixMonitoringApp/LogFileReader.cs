using System;
using System.IO;
using System.Text.RegularExpressions;

namespace _1C_ZabbixMonitoringApp
{
    public class LogFileReader
    {
        private StreamReader sr;
        private const string nextEventPattern = @"^(\d\d):(\d\d).(\d{3})\d+";
        private const string fileNamePattern = @"\\(\d\d)(\d\d)(\d\d)(\d\d)\.log";

        private string lastLine = String.Empty;
        private DateTime lastLineDT;

        private int year;
        private int month;
        private int day;
        private int hour;

        public DateTime CurrEventDT { get; set; }

        
        public LogFileReader(string path)
        {
            Match m = Regex.Match(path, fileNamePattern);
            year = int.Parse("20" + m.Groups[1].ToString());
            month = int.Parse(m.Groups[2].ToString());
            day = int.Parse(m.Groups[3].ToString());
            hour = int.Parse(m.Groups[4].ToString());

            sr = File.OpenText(@"" + path);

        }

        public string GetNextEvent()
        {
            string currLine = null;
            string currEvent = string.Empty;
            if (lastLine != String.Empty)
            {
                currEvent = lastLine;
                lastLine = String.Empty;

                CurrEventDT = lastLineDT;
            }

            while ((currLine = sr.ReadLine()) != null)
            {
                Match m = Regex.Match(currLine, nextEventPattern);
                if (m.Success)
                {
                    if (currEvent != String.Empty)
                    {
                        lastLine = currLine;
                        lastLineDT = new DateTime(year, month, day, hour,
                            int.Parse(m.Groups[1].ToString()),
                            int.Parse(m.Groups[2].ToString()),
                            int.Parse(m.Groups[3].ToString()));

                        return currEvent;
                    }
                    else
                    {
                        currEvent = currLine;
                        CurrEventDT = new DateTime(year, month, day, hour,
                            int.Parse(m.Groups[1].ToString()),
                            int.Parse(m.Groups[2].ToString()),
                            int.Parse(m.Groups[3].ToString()));
                    }
                }
                else
                {
                    if (currEvent.Length < 500000)
                        currEvent += currLine;
                }
            }
            return currEvent;
        }

    }
}
