﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using PrometheusFileServiceDiscovery.Contracts.Models;
using PrometheusFileServiceDiscoveryApi.Client;
using RestSharp;

namespace IISDiscoveryService.Synchronization.Persisters
{
    public class HostAsTargetPersister : IPersistHostAsTarget
    {
        public Dictionary<string,string> GlobalTags {get;set;}

        private readonly ITargetsClient _targetsClient;
        
        public HostAsTargetPersister(ITargetsClient targetsClient)
        {
            _targetsClient = targetsClient;
        }

        public IRestResponse Persist(string hostname, Dictionary<string,string> tags)
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

            var result = _targetsClient.Put(newTarget);

            return result;
        }       
    }
}