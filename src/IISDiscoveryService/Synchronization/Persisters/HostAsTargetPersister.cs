using System.Collections.Generic;
using PrometheusFileServiceDiscovery.Contracts.Models;
using PrometheusFileServiceDiscoveryApi.Client;
using RestSharp;

namespace IISDiscoveryService.Synchronization.Persisters
{
    public class HostAsTargetPersister : IPersistHostAsTarget
    {
        private readonly ITargetsClient _targetsClient;

        public HostAsTargetPersister(ITargetsClient targetsClient)
        {
            _targetsClient = targetsClient;
        }

        public IRestResponse Persist(string hostname)
        {
            var newTarget = new TargetModel()
            {
                Targets = new List<string>()
                {
                    hostname
                }
            };

            var result = _targetsClient.Put(newTarget);

            return result;
        }
    }
}