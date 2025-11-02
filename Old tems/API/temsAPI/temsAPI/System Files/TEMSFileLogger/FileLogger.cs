using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace temsAPI.System_Files.TEMSFileLogger
{
    public class FileLogger : ILogger
    {
        protected FileLoggerProvider FileLoggerProvider { get; private set; }

        public FileLogger([NotNull] FileLoggerProvider loggerProvider)
        {
            FileLoggerProvider = loggerProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && logLevel >= LogLevel.Error;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var logRecord = string.Format(
                "{0} [{1}] \n{2} \n{3}\n", 
                "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]", 
                logLevel.ToString(), 
                formatter(state, exception), 
                exception != null ? exception.StackTrace : "");

            LogRecord(logRecord);
        }
        
        public void Log<TState>(LogLevel logLevel, string message, params object[] args)
        {
            if (!IsEnabled(logLevel))
                return;

            //var logRecord = string.Format("[{0}] {1}", logLevel.ToString(), message);
            LogRecord(message);
        }

        private string GetFullFilePath()
        {
            return FileLoggerProvider.Settings.FolderPath + "/" + FileLoggerProvider.Settings.FilePath
                .Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyyMMdd"));
        }

        // BEFREEE => Not thread save (at all).
        private void LogRecord(string record)
        {
           using (var streamWriter = new StreamWriter(GetFullFilePath(), true))
           {
                    streamWriter.WriteLine(record);
           }
        }
    }
}
