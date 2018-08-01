using System.Collections.Generic;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Microsoft.Web.Administration;

namespace IISDiscoveryService.Providers
{
    public class IISWebsiteProvider
    {
        public List<string> Provide()
        {
            var result = new List<string>();
            var iisManager = new ServerManager();
            SiteCollection sites = iisManager.Sites;

            foreach (var site in sites)
            {
                foreach (var binding in site.Bindings)
                {
                    result.Add(binding.Host);
                }
            }

            return result;
        }
    }
}
