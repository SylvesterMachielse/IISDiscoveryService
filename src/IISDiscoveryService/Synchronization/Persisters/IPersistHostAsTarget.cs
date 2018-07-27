using RestSharp;
using System.Collections.Generic;

namespace IISDiscoveryService.Synchronization.Persisters
{
    public interface IPersistHostAsTarget
    {
        IRestResponse Persist(string hostname, Dictionary<string,string> tags);
    }
}