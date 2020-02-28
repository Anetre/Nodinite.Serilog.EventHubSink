using Nodinite.Serilog.Models;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;

namespace Nodinite.Serilog.EventHubSink
{
    public static class NodiniteEventHubSinkExtensions
    {
        public static LoggerConfiguration NodiniteEventHubSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  string ConnectionString, 
                  NodiniteLogEventSettings Settings,
                  IFormatProvider formatProvider = null,
                  LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException("loggerConfiguration");


            return loggerConfiguration.Sink(new NodiniteEventHubSink(ConnectionString, Settings, formatProvider), restrictedToMinimumLevel);
        }
    }
}
