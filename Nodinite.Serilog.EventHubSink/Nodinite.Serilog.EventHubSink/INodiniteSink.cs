using Nodinite.Serilog.Models;

namespace Nodinite.Serilog.EventHubSink
{
    interface INodiniteSink
    {
        void LogMessage(NodiniteLogEvent logEvent);
    }
}
