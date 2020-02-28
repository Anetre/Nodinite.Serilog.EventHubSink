using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Core;
using System;
using Nodinite.Serilog.EventHubSink;
using Nodinite.Serilog.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Nodinite.Serilog.EventHubSink.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReadSettingsFromAppSettingsTest()
        {
            // todo: implement moq
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Logger log = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            log.Information("Hello World");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitiateLogger_MissingLogAgentValue()
        {
            var connectionString = "";

            var settings = new NodiniteLogEventSettings()
            {
                EndPointDirection = 0,
                EndPointTypeId = 0,
                EndPointUri = "Nodinite.Serilog.Sink.Tests.Serilog",
                EndPointName = "Nodinite.Serilog.Sink.Tests",
                OriginalMessageTypeName = "Serilog.LogEvent",
                ProcessingUser = "NODINITE",
                ProcessName = "Nodinite.Serilog.Sink.Tests",
                ProcessingMachineName = "NODINITE-DEV",
                ProcessingModuleName = "DOTNETCORE.TESTS",
                ProcessingModuleType = "DOTNETCORE.TESTPROJECT"
            };

            Logger log = new LoggerConfiguration()
                .WriteTo.NodiniteEventHubSink(connectionString, settings)
                .CreateLogger();
        }

        [TestMethod]
        public void LogContextProperties()
        {
            var connectionString = "{YourEventHubAndQueueConnectionString}";

            var settings = new NodiniteLogEventSettings()
            {
                LogAgentValueId = 503,
                EndPointDirection = 0,
                EndPointTypeId = 0,
                EndPointUri = "Nodinite.Serilog.EventHubSink.Tests.Serilog",
                EndPointName = "Nodinite.Serilog.EventHubSink.Tests",
                ProcessingUser = "NODINITE",
                ProcessName = "Nodinite.Serilog.EventHubSink.Tests",
                ProcessingMachineName = "NODINITE-DEV",
                ProcessingModuleName = "DOTNETCORE.TESTS",
                ProcessingModuleType = "DOTNETCORE.TESTPROJECT"
            };

            ILogger log = new LoggerConfiguration()
                .WriteTo.NodiniteEventHubSink(connectionString, settings)
                .CreateLogger()
                .ForContext("ApplicationInterchangeId", $"CustomId-{Guid.NewGuid().ToString()}")
                .ForContext("CustomerId", 12)
                .ForContext("Body", JsonConvert.SerializeObject(new { Id = 1, Address = "Kalle kula veien 128" }))
                .ForContext("OriginalMessageType", "TestMessage#1.0");

            log.Information($"Customer '12' imported");
            log.Error("Customer not imported");
            log.Warning("Customer imported but some fields are not valid");
        }
    }

    public class TestMessage {
        public int Id { get; set; }
    }
}
