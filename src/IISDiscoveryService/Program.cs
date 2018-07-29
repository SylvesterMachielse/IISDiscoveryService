using System;
using System.Threading.Tasks;
using IISDiscoveryService.Running;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IISDiscoveryService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            bool adminPrivilegeAvailable = new AdminPrivilegeAvailableDeterminer().Determine();

            if (!adminPrivilegeAvailable)
            {
                throw new InvalidOperationException("No admin privilege available");
            }

            var hostBuilder = new HostBuilder()
                
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(configure => configure.AddConsole()).AddTransient<TimedHostedIISDiscoveryService>();
                    services.AddHostedService<TimedHostedIISDiscoveryService>();
                   
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}