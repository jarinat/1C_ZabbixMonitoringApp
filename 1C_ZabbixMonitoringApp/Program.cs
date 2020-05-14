using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace _1C_ZabbixMonitoringApp
{
    class Program
    {
        static int Main(string[] args)
        {
            #region FillParams
            Dictionary<string, CmdLineAgrDescr> argsDict = new Dictionary<string, CmdLineAgrDescr>
            {
                { "Help", new CmdLineAgrDescr{ Name = "--help", HasParameter = false} },
                { "Type", new CmdLineAgrDescr{ Name = "--type", HasParameter = true,
                    Descr = "Monitoring type. Available values: Timeout, Deadlock, Query, LockWaiting, Transaction"} },
                { "Path", new CmdLineAgrDescr{ Name = "--path", HasParameter = true,
                    Descr = "Log files path."} },
                { "MinDuration", new CmdLineAgrDescr{ Name = "--minDuration", HasParameter = true, Value = "0",
                    Descr = "Minimum event duration. This parameter doesn't count if equals 0. Default value 0"} },
                { "MaxDuration", new CmdLineAgrDescr{ Name = "--maxDuration", HasParameter = true, Value = "0",
                    Descr = "Maximum event duration. This parameter doesn't count if equals 0. Default value 0"} }
            };
            
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg == "--help")
                {
                    var argsDesrs = (from c in argsDict select c.Value).ToArray();
                    Console.WriteLine("Optons:");
                    foreach(var argsDesr in argsDesrs)
                    {
                        Console.WriteLine(argsDesr.ToString());
                    }
                    return 0;
                }

                var x = (from c in argsDict where c.Value.Name == arg select c).ToArray();
                if (x.Length == 0)
                {
                    Console.WriteLine("Parameter {0} wasn't found", arg);
                    return -1;
                }
                string key = x[0].Key;
                i++;
                argsDict[key].Value = args[i];
            }
            #endregion

            string monitoringType = argsDict["Type"].Value;
            string logPath = argsDict["Path"].Value;
            int minDuration = int.Parse(argsDict["MinDuration"].Value);
            int maxDuration = int.Parse(argsDict["MaxDuration"].Value);

            if (monitoringType == "")
            {
                Console.WriteLine("Monitoring type wasn't defined.");
                return -1;
            }

            if (logPath == "")
            {
                Console.WriteLine("Log path wasn't defined.");
                return -1;
            }

            DirectoryInfo dir = new DirectoryInfo(logPath);
            if (dir.Exists == false)
            {
                Console.WriteLine("Log path doesn't exist");
                return -1;
            }

            string regex = "";
            switch (monitoringType)
            {
                case "Timeout":
                    regex = @"^\d\d:\d\d\.(\d+)-(\d+),TTIMEOUT,";
                    break;
                case "Deadlock":
                    regex = @"^\d\d:\d\d\.(\d+)-(\d+),TDEADLOCK,";
                    break;
                case "LockWaiting":
                    regex = @"^\d\d:\d\d\.(\d+)-(\d+),TLOCK,";
                    break;
                case "Query":
                    regex = @"^\d\d:\d\d\.(\d+)-(\d+),(DBMSSQL|DBPOSTGRS),";
                    break;
                case "Transaction":
                    regex = @"^\d\d:\d\d\.(\d+)-(\d+),SDBL," +
                        @"((?:(?!(?:Func=Transaction))(?:.|\n))*)," +
                        @"Func=Transaction,Func=(RollbackTransaction|CommitTransaction)";
                    break;
                default:
                    Console.WriteLine("Unknown monitoring type");
                    return -1;
            }

            string currFilePath = String.Empty;
            string currEvent = String.Empty;
            int count = 0;

            LogFilesWalker excpFilesWalker = new LogFilesWalker(logPath);
            while ((currFilePath = excpFilesWalker.GetNextFilePath()) != String.Empty)
            {

                LogFileReader lfReader = new LogFileReader(currFilePath);

                while ((currEvent = lfReader.GetNextEvent()) != String.Empty)
                {
                    Match m = Regex.Match(currEvent, regex);
                    if (!m.Success)
                        continue;

                    int duration = int.Parse(m.Groups[2].ToString());

                    if (minDuration > 0 && minDuration >= duration)
                        continue;

                    if (maxDuration > 0 && maxDuration < duration)
                        continue;

                    count++;
                }
            }

            Console.WriteLine(count);
            
            Console.ReadLine();
            return 0;
        }
    }
}
