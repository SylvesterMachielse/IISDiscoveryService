using IISDiscoveryService.Synchronization.Models;

namespace IISDiscoveryService.Synchronization.Services
{
    public interface IApplySynchronizationRules
    {
        void Apply(SynchronizationRule rule);
    }
}