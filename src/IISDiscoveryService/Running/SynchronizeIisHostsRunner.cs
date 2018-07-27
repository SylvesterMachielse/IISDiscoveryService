using System;
using IISDiscoveryService.Synchronization.Services;
using Polly;

namespace IISDiscoveryService.Running
{
    public class SynchronizeIisHostsRunner
    {
        private readonly ISynchronizeIISHosts _iisHostSynchronizer;

        public SynchronizeIisHostsRunner(ISynchronizeIISHosts iisHostSynchronizer)
        {
            _iisHostSynchronizer = iisHostSynchronizer;
        }

        public void Run(object state)
        {
            Policy.Handle<Exception>().WaitAndRetryForever(
                SleepDurationProvider,
                OnRetry).Execute(
                _iisHostSynchronizer.Synchronize);
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
