using Autofac;
using IISDiscoveryService.Running;
using IISDiscoveryService.Synchronization.Factories;
using IISDiscoveryService.Synchronization.Models;
using IISDiscoveryService.Synchronization.Persisters;
using IISDiscoveryService.Synchronization.Providers;
using IISDiscoveryService.Synchronization.Services;
using IISDiscoveryService.SynchronizationProviders;
using PrometheusFileServiceDiscoveryApi.Client;

namespace IISDiscoveryService.DependencyInjection
{
    public class IisDiscoveryServiceContainerBuilder
    {
        public IContainer Build(Configuration configuration)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new TargetsClient("http://localhost:9099")).As<ITargetsClient>().SingleInstance();
            builder.RegisterType<HostNameProvider>().As<IProvideHostNames>().SingleInstance();
            builder.RegisterType<TargetsReflectingHostsProvider>().As<IProvideTargetsReflectingHosts>().SingleInstance();
            builder.Register(c => new HostAsTargetPersister(c.Resolve<ITargetsClient>()) {GlobalTags = configuration.GlobalTags}).As<IPersistHostAsTarget>().SingleInstance();
            builder.RegisterType<TargetDeleter>().As<IDeleteTargets>().SingleInstance();
            builder.RegisterType<SynchronizationRuleApplier>().As<IApplySynchronizationRules>().SingleInstance();
            builder.Register(c => new SynchronizeIisHostsRunner(c.Resolve<IApplySynchronizationRules>()) {Configuration = configuration }).AsSelf().SingleInstance();
            builder.Register(c => new TargetModelFactory() {GlobalTags = configuration.GlobalTags }).As<ICreateTargetModels>().SingleInstance();

            builder.RegisterType<MainService>().AsSelf().SingleInstance();

            return builder.Build();
        }
    }
}