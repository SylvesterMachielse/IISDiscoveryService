using System;
using IISDiscoveryService.Synchronization.Models;
using IISDiscoveryService.Synchronization.Services;
using Polly;

namespace IISDiscoveryService.Running
{
    public class SynchronizeIisHostsRunner
    {
        private readonly IApplySynchronizationRules _iisHostSynchronizer;
        
        public Configuration Configuration { get; set; }

        public SynchronizeIisHostsRunner(IApplySynchronizationRules iisHostSynchronizer)
        {
            _iisHostSynchronizer = iisHostSynchronizer;
        }

        public void Run(object state)
        {
            foreach (var rule in Configuration.SynchronizationRules)
            {
                _iisHostSynchronizer.Apply(rule);
            }
        }

        private void OnRetry(Exception exception, int arg2, TimeSpan arg3)
        {
            //TODO: up a metric
            Console.WriteLine($"EXCEPTION: {exception.Message}, {nameof(arg2)}: {arg2}, {nameof(arg3)}:{arg3}");
        }

        private TimeSpan SleepDurationProvider(int arg)
        {
            return TimeSpan.FromSeconds(60);
        }
    }
}
