using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Octopus.Client;
using Octopus.Client.Model;

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
        public void UploadModel(RoleModel model)
        {
            var knownRoles = model.Roles.Select(x => x.Name).ToList();

            var machines = _repository.Machines.FindAll();

            CheckMachinesExistence(machines.Select(x => x.Name).ToList(), model.Roles.SelectMany(x => x.Machines).Distinct(_comparer).ToList());

            Logger.Info("\n\tUploading roles");

            foreach (var machineResource in machines)
            {
                UploadRoleMappingForMachine(machineResource, model.Roles, knownRoles);
            }
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

        private void OutputMappingChanges(List<string> oldRoles, List<string> newRoles, List<string> notMappedRoles)
        {
            foreach (var newRole in newRoles)
            {
                Logger.Info($"- {newRole} {(oldRoles.Contains(newRole, _comparer) ? $"untouched {(notMappedRoles.Contains(newRole, _comparer) ? "(not mapped)" : string.Empty)}" : "added")}");
            }

            foreach (var oldRole in oldRoles.Where(oldRole => !newRoles.Contains(oldRole, _comparer)))
            {
                Logger.Info($"- {oldRole} deleted");
            }
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
