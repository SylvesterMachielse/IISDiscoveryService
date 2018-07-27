using System;
using System.Threading;
using Autofac;
using IISDiscoveryService.DependencyInjection;
using IISDiscoveryService.Running;
using IISDiscoveryService.Synchronization.Services;

namespace IISDiscoveryService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading configuration");

            var timer = new Timer(Run, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            Console.ReadKey();
        }

        private static void Run(object state)
        {
            var configuration = new ConfigurationProvider().Provide();

            using (var scope = new IisDiscoveryServiceContainerBuilder().Build(configuration))
            {
                var synchronizationRulesApplier = scope.Resolve<IApplySynchronizationRules>();
                
                foreach (var rule in configuration.SynchronizationRules)
                {
                    synchronizationRulesApplier.Apply(rule);
                }
            }
        }
    }
}