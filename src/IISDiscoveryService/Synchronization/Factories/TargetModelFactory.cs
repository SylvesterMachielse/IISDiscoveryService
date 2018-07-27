using System.Collections.Generic;
using System.Text.RegularExpressions;
using PrometheusFileServiceDiscovery.Contracts.Models;

namespace IISDiscoveryService.Synchronization.Factories
{
    public class TargetModelFactory : ICreateTargetModels
    {
        public Dictionary<string,string> GlobalTags {get;set;}

        public TargetModel Create(string hostname, Dictionary<string, string> tags)
        {
            var newTarget = new TargetModel()
            {
                Targets = new List<string>()
                {
                    hostname
                },
                Labels = new Dictionary<string, string>()
            };

            if (GlobalTags != null)
            {
                foreach(var kvp in GlobalTags)
                {
                    newTarget.Labels.Add(kvp.Key, kvp.Value);
                }
            }

            if(tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    newTarget.Labels.Add(tag.Key, Regex.Match(hostname, tag.Value).Value);
                }
            }

            return newTarget;
        }
    }
}