using PrometheusFileServiceDiscovery.Contracts.Models;
using PrometheusFileServiceDiscoveryApi.Client;
using RestSharp;

namespace IISDiscoveryService.Synchronization.Persisters
{
    public class TargetDeleter : IDeleteTargets
    {
        private readonly ITargetsClient _targetsClient;

        public TargetDeleter(ITargetsClient targetsClient)
        {
            _targetsClient = targetsClient;
        }

        public IRestResponse Delete(TargetModel target)
        {
            var result = _targetsClient.Delete(target.Targets[0]);

            return result;
        }
    }
}