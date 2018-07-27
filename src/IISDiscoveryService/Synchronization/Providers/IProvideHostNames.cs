using System.Collections.Generic;

namespace IISDiscoveryService.Synchronization.Providers
{
    public interface IProvideHostNames
    {
        List<string> Provide(string regexFilter);
    }
}