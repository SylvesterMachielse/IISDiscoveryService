using RestSharp;

namespace IISDiscoveryService.Synchronization.Persisters
{
    public interface IPersistHostAsTarget
    {
        IRestResponse Persist(string hostname);
    }
}