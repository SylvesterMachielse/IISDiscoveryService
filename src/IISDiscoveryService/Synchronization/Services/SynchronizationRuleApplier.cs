using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using IISDiscoveryService.Synchronization.Factories;
using IISDiscoveryService.Synchronization.Models;
using IISDiscoveryService.Synchronization.Persisters;
using IISDiscoveryService.Synchronization.Providers;
using PrometheusFileServiceDiscovery.Contracts.Models;
using PrometheusFileServiceDiscoveryApi.Client;
using IISDiscoveryService.Extensions;
using IISDiscoveryService.Running;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<TimedHostedIISDiscoveryService> _logger;
        
        public SynchronizationRuleApplier(
            IProvideHostNames hostNameProvider, 
            ITargetsClient targetsClient,
            IProvideTargetsReflectingHosts targetsReflectingHostProvider, 
            IPersistHostAsTarget hostAsTargetPersister,
            IDeleteTargets targetDeleter,
            ICreateTargetModels targetModelFactory, 
            ILogger<TimedHostedIISDiscoveryService> logger)
        {
            _hostNameProvider = hostNameProvider;
            _targetsClient = targetsClient;
            _targetsReflectingHostProvider = targetsReflectingHostProvider;
            _hostAsTargetPersister = hostAsTargetPersister;
            _targetDeleter = targetDeleter;
            _targetModelFactory = targetModelFactory;
            _logger = logger;
        }

        public void Apply(SynchronizationRule rule)
        {
            _logger.LogInformation($"Synchronizing rule: {rule.Name}");

            var hostsToProcess = GetHostsToProcess(rule.HostRegexFilter);
            _logger.LogInformation($"Current number of IIS hosts: {hostsToProcess.Count}");

            var targetsToProcess = GetTargetsToProcess(rule.HostRegexFilter);
            _logger.LogInformation($"Current number of metric targets: {targetsToProcess.Count}");

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
            _logger.LogInformation("Removing targets that have no corresponding host");

            foreach (var targetToProcess in targetsToProcess.Where(x => !x.Processed))
            {
                _targetDeleter.Delete(targetToProcess.Item);
                targetToProcess.Processed = true;

                _logger.LogInformation($"Target {targetToProcess.Item.Targets[0]} deleted");
            }
        }

        private void ProcessHostsInSync(List<ItemToProcess<string>> hostsToProcess, List<ItemToProcess<TargetModel>> targetsToProcess, Dictionary<string,string> tags)
        {
            _logger.LogInformation("Filtering up to date hosts");
          
            foreach (var hostToProcess in hostsToProcess)
            {
                var reflectingTarget = _targetsReflectingHostProvider.Provide(hostToProcess.Item,
                    targetsToProcess.Where(x => !x.Processed).ToList());

                if (reflectingTarget != null)
                {
                    var newTarget = _targetModelFactory.Create(hostToProcess.Item, tags);
                    if(!newTarget.Labels.ContentEquals(reflectingTarget.Tags)){ 
                        
                        _logger.LogInformation($"Host {hostToProcess.Item} is up to date, but tags are out of date");
                        _logger.LogInformation($"Patching tags of {hostToProcess.Item}");

                        _targetsClient.Patch(newTarget);
                    }
                    else
                    {
                        _logger.LogInformation($"Host {hostToProcess.Item} up to date");
                    }

                    reflectingTarget.Processed = true;
                    hostToProcess.Processed = true;
                }
            }
        }

        private void AddNewTargets(List<ItemToProcess<string>> hostsToProcess, Dictionary<string,string> tags)
        {
            var hostToProcesses = hostsToProcess.Where(x => !x.Processed).ToList();
            _logger.LogInformation($"Processing {hostToProcesses.Count} hosts with no corresponding scrape target");

            foreach (var hostToProcess in hostToProcesses)
            {
                _hostAsTargetPersister.Persist(hostToProcess.Item, tags);
                hostToProcess.Processed = true;

                _logger.LogInformation($"Host {hostToProcess.Item} added");
            }
        }

        private List<ItemToProcess<TargetModel>> GetTargetsToProcess(string hostRegexFilter)
        {
            var restResponse = _targetsClient.Get();
            if (restResponse.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"PrometheusFileServiceDiscoveryApi did not return status 200, instead it returned {restResponse.StatusCode}. Full response: {restResponse}");

                throw new InvalidOperationException("Cannot continue when PrometheusFileServiceDiscoveryApi is not available");
            }

            var targets = restResponse.Data;
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