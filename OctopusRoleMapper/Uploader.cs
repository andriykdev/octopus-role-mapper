using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusRoleMapper.Model;

namespace OctopusRoleMapper
{
    public class Uploader
    {
        private static readonly ILog Logger = LogManager.GetLogger<Uploader>();
        private readonly IOctopusRepository _repository;
        private readonly IEqualityComparer<string> _comparer = StringComparer.InvariantCultureIgnoreCase;

        public Uploader(string octopusUrl, string octopusApiKey) : this(new OctopusRepository(new OctopusClient(new OctopusServerEndpoint(octopusUrl, octopusApiKey))))
        {
        }

        public Uploader(IOctopusRepository repository)
        {
            _repository = repository;
        }

        // Machines will be updated with roles defined in the definitions folder.
        // If machine is missing defined role it will be added.
        // If machine has known(we have definition for it in repository) role, 
        // but in repo it doesn't point to this particular machine - it will be removed.
        // However we do not touch unknown roles (roles we are not aware of eg: they are not defined in a repository)
        // for tenants and tags logic is the same
        public void UploadModel(SystemModel model)
        {
            var knownRoles = model.Roles.Select(x => x.Name).ToList();
            var knownTenants = model.Tenants.Select(x => x.Name).ToList();
            var knownTags = model.TenantTags.Select(x => x.Name).ToList();

            var machines = _repository.Machines.FindAll();

            CheckMachinesExistence(machines.Select(x => x.Name).ToList(), model.Roles.SelectMany(x => x.Machines).Distinct(_comparer).ToList());
            Logger.Info("\n\tUploading roles");

            foreach (var machineResource in machines)
            {
                UploadRoleMappingForMachine(machineResource, model.Roles, knownRoles);
            }

            CheckMachinesExistence(machines.Select(x => x.Name).ToList(), model.Tenants.SelectMany(x => x.Machines).Distinct(_comparer).ToList());
            Logger.Info("\n\tUploading tenants");

            foreach (var machineResource in machines)
            {
                UploadTenantMappingForMachine(machineResource, model.Tenants, knownTenants);
            }

            CheckMachinesExistence(machines.Select(x => x.Name).ToList(), model.TenantTags.SelectMany(x => x.Machines).Distinct(_comparer).ToList());
            Logger.Info("\n\tUploading tags");

            foreach (var machineResource in machines)
            {
                UploadTenantTagsMappingForMachine(machineResource, model.TenantTags, knownTags);
            }
        }

        private void UploadTenantMappingForMachine(MachineResource machineResource, IEnumerable<Tenant> tenants, List<string> knownTenants)
        {
            var knownTenantIds = GetTenantIdsByNames(knownTenants);
            var unknownTenantIds = machineResource.TenantIds.Except(knownTenantIds, _comparer).ToList();

            var tenantsForMachine = tenants
                .Where(x => x.Machines.Contains(machineResource.Name, _comparer))
                .Select(x => x.Name)
                .ToList();

            var tenantIds = GetTenantIdsByNames(tenantsForMachine)
                .Concat(unknownTenantIds)
                .ToList();

            if (tenantIds.Count == 0)
            {
                return;
            }

            Logger.Info($"Updating {machineResource.Name} with tenants:");
            OutputMappingChanges(GetTenantNamesByIds(machineResource.TenantIds.ToList()), GetTenantNamesByIds(tenantIds), GetTenantNamesByIds(unknownTenantIds));

            machineResource.TenantIds = new ReferenceCollection(tenantIds);
            _repository.Machines.Modify(machineResource);
        }

        private void UploadTenantTagsMappingForMachine(MachineResource machineResource, IEnumerable<TenantTag> tenantTags, List<string> knownTags)
        {
            var unknownTags = machineResource.TenantTags.Except(knownTags, _comparer).ToList();
            var tagsForMachine = tenantTags
                .Where(x => x.Machines.Contains(machineResource.Name, _comparer))
                .Select(x => x.Name)
                .Concat(unknownTags)
                .ToList();

            if (tagsForMachine.Count == 0)
            {
                return;
            }

            Logger.Info($"Updating {machineResource.Name} with TenantTags:");
            OutputMappingChanges(machineResource.TenantTags.ToList(), tagsForMachine, unknownTags);

            machineResource.TenantTags = new ReferenceCollection(tagsForMachine);
            _repository.Machines.Modify(machineResource);
        }
       
        private void UploadRoleMappingForMachine(MachineResource machineResource, IEnumerable<Role> roles, IEnumerable<string> knownRoles)
        {
            var unknownRoles = machineResource.Roles.Except(knownRoles, _comparer).ToList();
            var rolesForMachine = roles
                .Where(x => x.Machines.Contains(machineResource.Name, _comparer))
                .Select(x => x.Name)
                .Concat(unknownRoles)
                .ToList();

            if (rolesForMachine.Count == 0)
            {
                throw new InvalidOperationException($"Machine {machineResource.Name} has no roles");
            }

            Logger.Info($"Updating {machineResource.Name} with roles:");
            OutputMappingChanges(machineResource.Roles.ToList(), rolesForMachine, unknownRoles);

            machineResource.Roles = new ReferenceCollection(rolesForMachine);
            _repository.Machines.Modify(machineResource);
        }

        private void OutputMappingChanges(IEnumerable<string> oldItems, IEnumerable<string> newItems, IEnumerable<string> notMappedItems)
        {
            foreach (var newItem in newItems)
            {
                Logger.Info($"- {newItem} {(oldItems.Contains(newItem, _comparer) ? $"untouched {(notMappedItems.Contains(newItem, _comparer) ? "(not mapped)" : string.Empty)}" : "added")}");
            }

            foreach (var oldItem in oldItems.Where(value => !newItems.Contains(value, _comparer)))
            {
                Logger.Info($"- {oldItem} deleted");
            }
        }

        private IEnumerable<string> GetTenantIdsByNames(IEnumerable<string> tenantsNames)
        {
            return _repository.Tenants.FindAll().Where(x => tenantsNames.Contains(x.Name, _comparer)).Select(x => x.Id).ToList();
        }

        private IEnumerable<string> GetTenantNamesByIds(IEnumerable<string> ids)
        {
            return _repository.Tenants.FindAll().Where(x => ids.Contains(x.Id, _comparer)).Select(x => x.Name).ToList();
        }

        private void CheckMachinesExistence(IEnumerable<string> remoteMachines, IEnumerable<string> localMachines)
        {
            var missedMachines = localMachines.Except(remoteMachines, _comparer).ToList();

            if (missedMachines.Any())
            {
                foreach (var missedMachine in missedMachines)
                {
                    Logger.ErrorFormat($"Machine {missedMachine} is missing on Octopus");
                }

                throw new InvalidOperationException("Local mappped machine(s) are missing on Octopus");
            }
        }
    }
}
