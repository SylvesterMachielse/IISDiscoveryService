using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using IISDiscoveryService.DependencyInjection;
using IISDiscoveryService.Synchronization.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IISDiscoveryService.Running
{
    public class TimedHostedIISDiscoveryService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedIISDiscoveryService> _logger;
        private Timer _timer;

        public TimedHostedIISDiscoveryService(ILogger<TimedHostedIISDiscoveryService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("IISDiscoveryService is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("IISDiscoveryService is working.");

            var configuration = new ConfigurationProvider().Provide();

            using (var scope = new IisDiscoveryServiceContainerBuilder().Build(configuration, _logger))
            {
                var synchronizationRulesApplier = scope.Resolve<IApplySynchronizationRules>();

                foreach (var rule in configuration.SynchronizationRules)
                {
                    synchronizationRulesApplier.Apply(rule);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("IISDiscoveryService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}