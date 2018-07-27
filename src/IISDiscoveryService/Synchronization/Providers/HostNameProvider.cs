using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Web.Administration;

namespace IISDiscoveryService.Synchronization.Providers
{
    public class HostNameProvider : IProvideHostNames
    {
        public List<string> Provide(string regexFilter)
        {
            var result = new List<string>();
            var iisManager = new ServerManager();
            SiteCollection sites = iisManager.Sites;

            foreach (Site site in sites)
            {
                foreach (var binding in site.Bindings)
                {
                    if (!Regex.IsMatch(binding.Host, regexFilter)) continue;

                    result.Add(binding.Host);
                }
            }

            return result;
        }
    }
}