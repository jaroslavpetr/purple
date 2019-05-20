using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;


namespace Watchdog
{
    public class common
    {

        public static NLog.Logger logger;

        //Start new log process
        public static void StartLogging()
        {

            NLog.Config.LoggingConfiguration NlogCoinfig = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("server_monitor") { FileName = @"Logs\ServerMonitor.txt" };

            NlogCoinfig.AddRule(LogLevel.Trace, LogLevel.Debug, logfile, "server_monitor");
            NlogCoinfig.AddRule(LogLevel.Info, LogLevel.Warn, logfile, "server_monitor");
            NlogCoinfig.AddRule(LogLevel.Error, LogLevel.Fatal, logfile, "server_monitor");
            
            NlogCoinfig.AddTarget(logfile);
            
            NLog.LogManager.Configuration = NlogCoinfig;
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

        //Create new entry into log file
        public static void DisplayAndLog(string what, string how = "info")
        {
            var task1Logger = NLog.LogManager.GetLogger("server_monitor");
            if (how == "info") task1Logger.Info(what);
            if (how == "error") task1Logger.Error(what);
            Console.WriteLine(what);

        }
    }
}
