using PrometheusFileServiceDiscovery.Contracts.Models;
using RestSharp;

namespace IISDiscoveryService.Synchronization.Persisters
{
    public interface IDeleteTargets
    {
        IRestResponse Delete(TargetModel target);
    }
}