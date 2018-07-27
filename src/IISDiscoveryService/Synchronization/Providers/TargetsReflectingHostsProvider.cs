using System;
using System.Collections.Generic;
using System.Linq;
using IISDiscoveryService.Synchronization.Models;
using IISDiscoveryService.Synchronization.Providers;
using PrometheusFileServiceDiscovery.Contracts.Models;

namespace IISDiscoveryService.SynchronizationProviders
{
    public class TargetsReflectingHostsProvider : IProvideTargetsReflectingHosts
    {
        public ItemToProcess<TargetModel> Provide(string hostName, List<ItemToProcess<TargetModel>> targets)
        {
            if(!targets.Any()) return null;

            var result = targets.SingleOrDefault(x =>
                x.Item.Targets.Any(y => y.Equals(hostName, StringComparison.InvariantCultureIgnoreCase)));

            return result;
        }
    }
}