using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Services;

namespace temsAPI.System_Files.TEMSFileLogger
{
    public class FileLoggerProvider : ILoggerProvider
    {
        public FileLoggerSettings Settings { get; private set; }

        public FileLoggerProvider(IOptions<FileLoggerSettings> settings)
        {
            Settings = settings.Value;

            if (!Directory.Exists(Settings.FolderPath))
            {
                Directory.CreateDirectory(Settings.FolderPath);
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(this);
        }

        public void Dispose()
        {
        }
    }
}
