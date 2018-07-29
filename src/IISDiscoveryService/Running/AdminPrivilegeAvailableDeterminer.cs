using System.Security.Principal;
using PrometheusFileServiceDiscoveryApi.Client;

namespace IISDiscoveryService.Running
{
    public class AdminPrivilegeAvailableDeterminer
    {
        public bool Determine()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            return isElevated;
        }
    }
}