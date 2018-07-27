using System;
using System.Collections.Generic;
using Microsoft.Web.Administration;

namespace IISDiscoveryService.Synchronization.Providers
{
    public class HostNameProvider : IProvideHostNames
    {
        public List<string> Provide()
        {
            var result = new List<string>();
            var iisManager = new ServerManager();
            SiteCollection sites = iisManager.Sites;

            foreach (Site site in sites)
            {
                Console.WriteLine(site.Name);

                foreach (var binding in site.Bindings)
                {
                    result.Add(binding.Host);
                }
            }

            return result;
        }
    }
}