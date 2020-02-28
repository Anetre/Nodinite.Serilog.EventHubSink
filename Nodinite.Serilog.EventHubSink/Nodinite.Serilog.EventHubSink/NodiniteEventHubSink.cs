using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Nodinite.Serilog.Models;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Nodinite.Serilog.EventHubSink
{
    public class NodiniteEventHubSink : ILogEventSink, INodiniteSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly string _connectionString;
        private readonly NodiniteLogEventSettings _settings;
        private readonly Guid _localInterchangeId;

        EventHubClient _client;

        public NodiniteEventHubSink(string connectionString, NodiniteLogEventSettings settings, IFormatProvider formatProvider)
        {
            _connectionString = connectionString;
            _settings = settings;
            _formatProvider = formatProvider;
            _localInterchangeId = Guid.NewGuid();

            // validate settings
            if (!_settings.LogAgentValueId.HasValue)
                throw new ArgumentNullException("LogAgentValueId must not be null");
            
            _client = EventHubClient.CreateFromConnectionString(connectionString);
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);

            var nEvent = new NodiniteLogEvent(message, logEvent, _settings);

            LogMessage(nEvent);
        }

        public void LogMessage(NodiniteLogEvent logEvent)
        {
            logEvent.LocalInterchangeId = _localInterchangeId;
            logEvent.ServiceInstanceActivityId = Guid.NewGuid();


            _client = EventHubClient.CreateFromConnectionString(_connectionString);

            var message = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(logEvent)));

            _client.SendAsync(message).GetAwaiter().GetResult();
        }
    }
}
