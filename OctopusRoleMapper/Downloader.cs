using System;
using System.Linq;
using Octopus.Client;

namespace OctopusRoleMapper
{
    public class Downloader
    {
        private readonly IOctopusRepository _repository;

        public Downloader(string octopusUrl, string octopusApiKey) : this(new OctopusRepository(new OctopusClient(new OctopusServerEndpoint(octopusUrl, octopusApiKey))))
        {
        }

        public Downloader(IOctopusRepository repository)
        {
            _repository = repository;
        }

        public RoleModel DownloadModel()
        {
            var machines = _repository.Machines.FindAll();
            var roles = _repository.MachineRoles.GetAllRoleNames();

            return new RoleModel(roles.Select(role => new Role(role, machines.Where(x => x.Roles.Contains(role, StringComparer.CurrentCultureIgnoreCase)).Select(x => x.Name))));
        }
    }
}
