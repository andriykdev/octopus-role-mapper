using System;
using System.Linq;
using Octopus.Client;
using OctopusRoleMapper.Model;

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

        public SystemModel DownloadModel()
        {
            var machines = _repository.Machines.FindAll();
            var roles = _repository.MachineRoles.GetAllRoleNames();
            var tenants = _repository.Tenants.FindAll();
            var tags = _repository.TagSets.FindAll().SelectMany(x => x.Tags);

            return new SystemModel(
                roles.Select(role => new Role(role, machines.Where(x => x.Roles.Contains(role, StringComparer.CurrentCultureIgnoreCase)).Select(x => x.Name))),
                tenants.Select(tenant => new Tenant(tenant.Name, machines.Where(x => x.TenantIds.Contains(tenant.Id)).Select(x => x.Name))),
                tags.Select(tag => new TenantTag(tag.CanonicalTagName, machines.Where(x => x.TenantTags.Contains(tag.CanonicalTagName)).Select(x => x.Name))));
        }
    }
}
