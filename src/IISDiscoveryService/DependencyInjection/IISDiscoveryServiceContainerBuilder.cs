
using Autofac;
using IISDiscoveryService.Running;
using IISDiscoveryService.Synchronization.Persisters;
using IISDiscoveryService.Synchronization.Providers;
using IISDiscoveryService.Synchronization.Services;
using IISDiscoveryService.SynchronizationProviders;
using PrometheusFileServiceDiscoveryApi.Client;

namespace IISDiscoveryService.DependencyInjection
{
   public  class IisDiscoveryServiceContainerBuilder
    {
        public IContainer  Build()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new TargetsClient("http://localhost:9099")).As<ITargetsClient>().SingleInstance();
            builder.RegisterType<HostNameProvider>().As<IProvideHostNames>().SingleInstance();
            builder.RegisterType<TargetsReflectingHostsProvider>().As<IProvideTargetsReflectingHosts>().SingleInstance();
            builder.RegisterType<HostAsTargetPersister>().As<IPersistHostAsTarget>().SingleInstance();
            builder.RegisterType<TargetDeleter>().As<IDeleteTargets>().SingleInstance();
            builder.RegisterType<SynchronizeIisHosts>().As<ISynchronizeIISHosts>().SingleInstance();
            builder.RegisterType<SynchronizeIisHostsRunner>().AsSelf().SingleInstance();
            builder.RegisterType<MainService>().AsSelf().SingleInstance();
            
            return builder.Build();
        }
    }
}
