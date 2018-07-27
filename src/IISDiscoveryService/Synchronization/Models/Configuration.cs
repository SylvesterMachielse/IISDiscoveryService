using System;
using System.Collections.Generic;
using System.Text;

namespace IISDiscoveryService.Synchronization.Models
{
   public  class Configuration
    {
        public Dictionary<string, string> GlobalTags { get; set; }
        public List<SynchronizationRule> SynchronizationRules { get;set; }
    }

    public class SynchronizationRule
    {
        public string Name { get;set; }
        public string HostRegexFilter { get; set; }
        public Dictionary<string, string> TargetLabels { get; set; }
    }
}
