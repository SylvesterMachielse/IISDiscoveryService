using System.Collections.Generic;
using IISDiscoveryService.Synchronization.Models;
using PrometheusFileServiceDiscovery.Contracts.Models;

namespace IISDiscoveryService.Synchronization.Providers
{
    public interface IProvideTargetsReflectingHosts
    {
        ItemToProcess<TargetModel> Provide(string hostName, List<ItemToProcess<TargetModel>> targets);
    }
}