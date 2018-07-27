using System;
using System.Threading;
using IISDiscoveryService.Running;

namespace IISDiscoveryService
{
    public class MainService
    {
        private readonly SynchronizeIisHostsRunner _synchronizeIisHostsRunner;

        public MainService(SynchronizeIisHostsRunner iisHostSynchronizer)
        {
            _synchronizeIisHostsRunner = iisHostSynchronizer;
        }

        public void Run()
        {
            var timer = new Timer(_synchronizeIisHostsRunner.Run, "", TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1));
        }
    }
}