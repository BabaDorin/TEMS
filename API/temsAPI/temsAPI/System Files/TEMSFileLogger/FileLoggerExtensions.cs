using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.System_Files.TEMSFileLogger
{
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerSettings> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
