using System.Collections.Generic;
using IISDiscoveryService.Synchronization.Models;
using PrometheusFileServiceDiscovery.Contracts.Models;

namespace IISDiscoveryService.Synchronization
{
    public interface IProvideHostsReflectingTargets
    {
        ItemToProcess<string> Provide(TargetModel target, List<ItemToProcess<string>> hosts);
    }
}