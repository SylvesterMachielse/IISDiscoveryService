using System;
using Autofac;
using IISDiscoveryService.DependencyInjection;

namespace IISDiscoveryService
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var scope = new IisDiscoveryServiceContainerBuilder().Build())
            {
                var mainService = scope.Resolve<MainService>();
                mainService.Run();
            
            }

            Console.ReadKey();
        }

        private static void OnRetry(Exception exception, int arg2, TimeSpan arg3)
        {
            //TODO: up a metric
            Console.WriteLine($"EXCEPTION: {exception.Message}, {nameof(arg2)}: {arg2}, {nameof(arg3)}:{arg3}");
        }

        private static TimeSpan SleepDurationProvider(int arg)
        {
            return TimeSpan.FromSeconds(10);
        }
    }
}
