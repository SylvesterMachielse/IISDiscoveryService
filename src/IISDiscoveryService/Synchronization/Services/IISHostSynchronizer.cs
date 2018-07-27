using System;
using System.Collections.Generic;
using System.Linq;
using IISDiscoveryService.Synchronization.Models;
using IISDiscoveryService.Synchronization.Persisters;
using IISDiscoveryService.Synchronization.Providers;
using PrometheusFileServiceDiscovery.Contracts.Models;
using PrometheusFileServiceDiscoveryApi.Client;

namespace IISDiscoveryService.Synchronization.Services
{
    public class SynchronizeIisHosts : ISynchronizeIISHosts
    {
        private readonly IProvideHostNames _hostNameProvider;
        private readonly ITargetsClient _targetsClient;
        private readonly IProvideTargetsReflectingHosts _targetsReflectingHostProvider;
        private readonly IPersistHostAsTarget _hostAsTargetPersister;
        private readonly IDeleteTargets _targetDeleter;


        public SynchronizeIisHosts(
            IProvideHostNames hostNameProvider, 
            ITargetsClient targetsClient,
            IProvideTargetsReflectingHosts targetsReflectingHostProvider, 
            IPersistHostAsTarget hostAsTargetPersister,
            IDeleteTargets targetDeleter)
        {
            _hostNameProvider = hostNameProvider;
            _targetsClient = targetsClient;
            _targetsReflectingHostProvider = targetsReflectingHostProvider;
            _hostAsTargetPersister = hostAsTargetPersister;
            _targetDeleter = targetDeleter;
        }

        public void Synchronize()
        {
            Console.WriteLine("Synchronizing...");

            var hostsToProcess = GetHostsToProcess();
            var targetsToProcess = GetTargetsToProcess();

            Process(hostsToProcess, targetsToProcess);
        }

        private void Process(List<ItemToProcess<string>> hostsToProcess, List<ItemToProcess<TargetModel>> targetsToProcess)
        {
            //METHOD SEQUENCE MATTERS!!

            //Items in sync ==> change nothing
            ProcessHostsInSync(hostsToProcess, targetsToProcess);

            //No corresponding target ==> persist new target
            AddNewTargets(hostsToProcess);

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

        private void AddNewTargets(List<ItemToProcess<string>> hostsToProcess)
        {
            foreach (var hostToProcess in hostsToProcess.Where(x => !x.Processed))
            {
                _hostAsTargetPersister.Persist(hostToProcess.Item);
                hostToProcess.Processed = true;

                Console.WriteLine($"Host {hostToProcess.Item} added");
            }
        }

        private void ProcessHostsInSync(List<ItemToProcess<string>> hostsToProcess,
            List<ItemToProcess<TargetModel>> targetsToProcess)
        {
            foreach (var hostToProcess in hostsToProcess)
            {
                var reflectingTarget = _targetsReflectingHostProvider.Provide(hostToProcess.Item,
                    targetsToProcess.Where(x => !x.Processed).ToList());

                if (reflectingTarget != null)
                {
                    reflectingTarget.Processed = true;
                    hostToProcess.Processed = true;

                    Console.WriteLine($"Host {hostToProcess.Item} up to date");
                }
            }
        }


        private List<ItemToProcess<TargetModel>> GetTargetsToProcess()
        {
            //TODO: handle request errors
            var targets = _targetsClient.Get().Data;
            var targetsToProcess = new List<ItemToProcess<TargetModel>>();
            targets.ForEach(x => targetsToProcess.Add(new ItemToProcess<TargetModel>()
            {
                Item = x
            }));
            return targetsToProcess;
        }

        private List<ItemToProcess<string>> GetHostsToProcess()
        {
            var hostNames = _hostNameProvider.Provide();

            var hostsToProcess = new List<ItemToProcess<string>>();
            hostNames.ForEach(x => hostsToProcess.Add(new ItemToProcess<string>()
            {
                Item = x
            }));
            return hostsToProcess;
        }
    }
}