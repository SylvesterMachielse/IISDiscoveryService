using System;
using System.IO;
using IISDiscoveryService.Synchronization.Models;
using Newtonsoft.Json;

namespace IISDiscoveryService.Running
{
    public class ConfigurationProvider
    {
        public Configuration Provide()
        {
            var settingsPath = Path.Combine(Environment.CurrentDirectory, "appsettings.json");

            var configurationJson = File.ReadAllText(settingsPath);

            var configuration = JsonConvert.DeserializeObject<Configuration>(configurationJson);

            return configuration;
        }
    }
}