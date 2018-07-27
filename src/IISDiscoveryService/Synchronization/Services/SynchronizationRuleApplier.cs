using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IISDiscoveryService.Synchronization.Factories;
using IISDiscoveryService.Synchronization.Models;
using IISDiscoveryService.Synchronization.Persisters;
using IISDiscoveryService.Synchronization.Providers;
using PrometheusFileServiceDiscovery.Contracts.Models;
using PrometheusFileServiceDiscoveryApi.Client;
using IISDiscoveryService.Extensions;

namespace IISDiscoveryService.Synchronization.Services
{
    public class SynchronizationRuleApplier : IApplySynchronizationRules
    {
        private readonly IProvideHostNames _hostNameProvider;
        private readonly ITargetsClient _targetsClient;
        private readonly IProvideTargetsReflectingHosts _targetsReflectingHostProvider;
        private readonly IPersistHostAsTarget _hostAsTargetPersister;
        private readonly IDeleteTargets _targetDeleter;
        private readonly ICreateTargetModels _targetModelFactory;
        
        public SynchronizationRuleApplier(
            IProvideHostNames hostNameProvider, 
            ITargetsClient targetsClient,
            IProvideTargetsReflectingHosts targetsReflectingHostProvider, 
            IPersistHostAsTarget hostAsTargetPersister,
            IDeleteTargets targetDeleter,
            ICreateTargetModels targetModelFactory)
        {
            _hostNameProvider = hostNameProvider;
            _targetsClient = targetsClient;
            _targetsReflectingHostProvider = targetsReflectingHostProvider;
            _hostAsTargetPersister = hostAsTargetPersister;
            _targetDeleter = targetDeleter;
            _targetModelFactory = targetModelFactory;
        }

        public void Apply(SynchronizationRule rule)
        {
            Console.WriteLine($"Synchronizing rule: {rule.Name}");

            var hostsToProcess = GetHostsToProcess(rule.HostRegexFilter);
            Console.WriteLine($"Current number of IIS hosts: {hostsToProcess.Count}");

            var targetsToProcess = GetTargetsToProcess(rule.HostRegexFilter);
            Console.WriteLine($"Current number of metric targets: {targetsToProcess.Count}");

            Process(hostsToProcess, targetsToProcess, rule.TargetLabels);
        }       

        private void Process(List<ItemToProcess<string>> hostsToProcess, List<ItemToProcess<TargetModel>> targetsToProcess, Dictionary<string,string> tags)
        {
            //METHOD SEQUENCE MATTERS!!

            //Items in sync ==> change nothing
            ProcessHostsInSync(hostsToProcess, targetsToProcess, tags);

            //No corresponding target ==> persist new target
            AddNewTargets(hostsToProcess, tags);

            //No corresponding host ==> delete target
            DeleteOldTargets(targetsToProcess);
        }

        private void DeleteOldTargets(List<ItemToProcess<TargetModel>> targetsToProcess)
        {
            foreach (var targetToProcess in targetsToProcess.Where(x => !x.Processed))
            {
                _targetDeleter.Delete(targetToProcess.Item);
                targetToProcess.Processed = true;

                Console.WriteLine($"Target {targetToProcess.Item.Targets[0]} deleted");
            }
        }

        private void AddNewTargets(List<ItemToProcess<string>> hostsToProcess, Dictionary<string,string> tags)
        {
            foreach (var hostToProcess in hostsToProcess.Where(x => !x.Processed))
            {
                _hostAsTargetPersister.Persist(hostToProcess.Item, tags);
                hostToProcess.Processed = true;

                Console.WriteLine($"Host {hostToProcess.Item} added");
            }
        }

        private void ProcessHostsInSync(List<ItemToProcess<string>> hostsToProcess, List<ItemToProcess<TargetModel>> targetsToProcess, Dictionary<string,string> tags)
        {

            //TODO: PATCH labels if needed
            foreach (var hostToProcess in hostsToProcess)
            {
                var reflectingTarget = _targetsReflectingHostProvider.Provide(hostToProcess.Item,
                    targetsToProcess.Where(x => !x.Processed).ToList());

                if (reflectingTarget != null)
                {
                    var newTarget = _targetModelFactory.Create(hostToProcess.Item, tags);
                    if(!newTarget.Labels.ContentEquals(reflectingTarget.Tags)){ 
                        
                        Console.WriteLine($"Patching tags of {hostToProcess.Item}");

                        _targetsClient.Patch(newTarget);
                        }
                        


                    reflectingTarget.Processed = true;
                    hostToProcess.Processed = true;

                    Console.WriteLine($"Host {hostToProcess.Item} up to date");
                }
            }
        }


        private List<ItemToProcess<TargetModel>> GetTargetsToProcess(string hostRegexFilter)
        {
            //TODO: handle request errors
            var targets = _targetsClient.Get().Data;
            var targetsToProcess = new List<ItemToProcess<TargetModel>>();
            targets.Where(t => Regex.IsMatch(t.Targets[0], hostRegexFilter)).ToList().ForEach(x => targetsToProcess.Add(new ItemToProcess<TargetModel>()
            {
                Item = x,
                Tags = x.Labels
            }));

            return targetsToProcess;
        }

        private List<ItemToProcess<string>> GetHostsToProcess(string hostRegexFilter)
        {
            var hostNames = _hostNameProvider.Provide(hostRegexFilter);

            var hostsToProcess = new List<ItemToProcess<string>>();
            hostNames.ForEach(x => hostsToProcess.Add(new ItemToProcess<string>()
            {
                Item = x
                
            }));
            return hostsToProcess;
        }
    }
}