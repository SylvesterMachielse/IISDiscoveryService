using System.Collections.Generic;
using PrometheusFileServiceDiscovery.Contracts.Models;

namespace IISDiscoveryService.Synchronization.Factories
{
    public interface ICreateTargetModels
    {
        TargetModel Create(string hostname, Dictionary<string, string> tags);
    }
}