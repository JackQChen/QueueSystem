
using System;
using System.IO;
using log4net;

namespace SoundPlayer
{
    public static class LogService
    {
        private static ILog debugLog, errorLog, fatalLog, infoLog, warnLog, dataLog;

        static LogService()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "LogConfig.xml";
            if (File.Exists(configPath))
                log4net.Config.XmlConfigurator.Configure(new FileInfo(configPath));
            debugLog = log4net.LogManager.GetLogger("DebugLogger");
            errorLog = log4net.LogManager.GetLogger("ErrorLogger");
            fatalLog = log4net.LogManager.GetLogger("FatalLogger");
            infoLog = log4net.LogManager.GetLogger("InfoLogger");
            warnLog = log4net.LogManager.GetLogger("WarnLogger");
            dataLog = log4net.LogManager.GetLogger("DataAccessLogger");
        }

        public static void Debug(string log)
        {
            if (debugLog.IsDebugEnabled)
                debugLog.Debug(log);
        }

        public static void Error(string log)
        {
            if (errorLog.IsErrorEnabled)
                errorLog.Error(log);
        }

        public static void Fatal(string log)
        {
            if (fatalLog.IsFatalEnabled)
                fatalLog.Fatal(log);
        }

        public static void Info(string log)
        {
            if (infoLog.IsInfoEnabled)
                infoLog.Info(log);
        }

        public static void Warn(string log)
        {
            if (warnLog.IsWarnEnabled)
                warnLog.Warn(log);
        }

        public static void DataAccess(string log)
        {
            if (dataLog.IsInfoEnabled)
                dataLog.Info(log);
        }

    }
}
