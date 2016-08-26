using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Octopus.Client;

namespace OctopusRoleMapper
{
    public class Uploader
    {
        private static readonly ILog Logger = LogManager.GetLogger<Uploader>();
        private readonly IOctopusRepository _repository;

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

            CheckMachinesExistence(machines.Select(x => x.Name).ToList(), model.Roles.SelectMany(x => x.Machines).Distinct().ToList());

            foreach (var machineResource in machines)
            {
                var unknownRoles = machineResource.Roles.Except(knownRoles).ToList();
                var rolesForMachine = model.Roles
                    .Where(x => x.Machines.Contains(machineResource.Name))
                    .Select(x => x.Name)
                    .ToList()
                    .Concat(unknownRoles)
                    .ToList();

                machineResource.Roles.Clear();

                Logger.Info($"Updating {machineResource.Name} with roles:");

                foreach (var role in rolesForMachine)
                {
                    Logger.Info($"- {role}");
                    machineResource.Roles.Add(role);
                }

                if (machineResource.Roles.Count == 0)
                {
                    throw new InvalidOperationException($"Machine {machineResource.Name} has no roles");
                }

                _repository.Machines.Modify(machineResource);
            }
        }

        private void CheckMachinesExistence(List<string> remoteMachines, List<string> localMachines)
        {
            foreach (var localMachine in localMachines)
            {
                if (!remoteMachines.Contains(localMachine))
                {
                    throw new InvalidOperationException($"Machine {localMachine} is missing on Octopus");
                }
            }
        }
    }
}
