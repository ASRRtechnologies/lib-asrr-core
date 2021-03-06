using System;
using System.IO;
using System.Linq;
using ASRR.Log;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ASRR.Core.Logs
{
    public static class LogHandler
    {
        private static Logger log;

        public static void AddLogTarget(NLogBasedLogConfiguration configuration)
        {
            try
            {
                var fileInfo = new FileInfo(configuration.LogFilePath);
                if (fileInfo.DirectoryName != null)
                    Directory.CreateDirectory(fileInfo.DirectoryName);

                if (!File.Exists(configuration.LogFilePath))
                    File.Create(configuration.LogFilePath);

                if (File.ReadAllLines(configuration.LogFilePath).ToList().Count() >
                    10000) //Archive file with timestamp, delete old file
                {
                    var date = DateTime.Now.ToString().Replace('/', '-');
                    date = date.Replace(':', '_');
                    var dirPath = Path.GetDirectoryName(configuration.LogFilePath);
                    var fileName = Path.GetFileName(configuration.LogFilePath);
                    fileName = fileName.Replace(".log", date + ".log");
                    File.Copy(configuration.LogFilePath, Path.Combine(dirPath, fileName));
                    File.Delete(configuration.LogFilePath);
                    File.Create(configuration.LogFilePath);
                }
            }
            catch
            {
                //Fail silently
            }

            configuration.OpenOnButton = true;
            LoggingConfiguration logConfiguration = NLog.LogManager.Configuration ?? new LoggingConfiguration();
            logConfiguration.RemoveTarget(configuration.LogName);
            FileTarget target = new FileTarget(configuration.LogName)
            {
                FileName = configuration.LogFilePath
            };

            logConfiguration.AddTarget(target);

            LogLevel level;
            switch (configuration.MinLevel)
            {
                case "Debug":
                    level = LogLevel.Debug;
                    break;
                case "Trace":
                    level = LogLevel.Trace;
                    break;
                case "Info":
                    level = LogLevel.Info;
                    break;
                case "Warn":
                    level = LogLevel.Warn;
                    break;
                case "Error":
                    level = LogLevel.Error;
                    break;
                default:
                    level = LogLevel.Trace;
                    break;
            }

            LoggingRule loggingRule = new LoggingRule(configuration.NameFilter, level, target);
            logConfiguration.LoggingRules.Add(loggingRule);

            NLog.LogManager.Configuration = logConfiguration;
            log = NLog.LogManager.GetCurrentClassLogger();
            log.Debug("Using programmatic config");
        }

        public static void RemoveLogTarget(string name)
        {
            LoggingConfiguration logConfiguration = NLog.LogManager.Configuration;
            logConfiguration?.RemoveTarget(name);
        }

        public static void AddDefaultTempLogTarget(string name, string logFilePath)
        {
            NLogBasedLogConfiguration defaultConfiguration = new NLogBasedLogConfiguration
            {
                OpenOnStartUp = false,
                LogName = name,
                LogFilePath = logFilePath,
                LogLayout = "${date:format=yyyy-MM-dd hh\\:mm\\:ss} | ${level} | ${logger} | ${message} ${exception}",
                NameFilter = "*",
                MinLevel = "Trace"
            };

            AddLogTarget(defaultConfiguration);
        }

        public static void AddErrorReportTempLogTarget(string name, string logFilePath)
        {
            NLogBasedLogConfiguration errorReportConfiguration = new NLogBasedLogConfiguration
            {
                OpenOnStartUp = false,
                LogName = name,
                LogFilePath = logFilePath,
                LogLayout = "${level} | ${message} ${exception}",
                NameFilter = "*",
                MinLevel = "Warn"
            };

            AddLogTarget(errorReportConfiguration);
        }
    }
}