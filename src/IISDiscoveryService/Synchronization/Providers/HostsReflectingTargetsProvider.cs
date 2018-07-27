using System;
using System.Collections.Generic;
using System.Linq;
using IISDiscoveryService.Synchronization.Models;
using PrometheusFileServiceDiscovery.Contracts.Models;

namespace IISDiscoveryService.Synchronization.Providers
{
    public class HostsReflectingTargetsProvider : IProvideHostsReflectingTargets
    {
        public ItemToProcess<string> Provide(TargetModel target, List<ItemToProcess<string>> hosts)
        {
            var result = hosts.SingleOrDefault(host =>
                target.Targets.Any(y => y.Equals(host.Item, StringComparison.InvariantCultureIgnoreCase)));

            return result;
        }
    }
}