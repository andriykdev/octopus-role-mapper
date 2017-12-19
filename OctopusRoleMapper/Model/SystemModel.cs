using System.Collections.Generic;
using System.Linq;

namespace OctopusRoleMapper.Model
{
    public class SystemModel
    {
        public IEnumerable<Role> Roles { get; }
        public IEnumerable<Tenant> Tenants { get; }
        public IEnumerable<TenantTag> TenantTags { get; }

        public SystemModel(IEnumerable<Role> roles, IEnumerable<Tenant> tenants, IEnumerable<TenantTag> tenantTags)
        {
            Roles = roles.OrderBy(x => x.Name).ToArray();
            Tenants = tenants.OrderBy(x => x.Name).ToArray();
            TenantTags = tenantTags.OrderBy(x => x.Name).ToArray();
        }

        public IEnumerable<SystemModel> SplitModel()
        {
            return Roles.Select(r => new SystemModel(Enumerable.Repeat(r, 1), Enumerable.Empty<Tenant>(), Enumerable.Empty<TenantTag>()))
                .Concat(Tenants.Select(t => new SystemModel(Enumerable.Empty<Role>(), Enumerable.Repeat(t, 1), Enumerable.Empty<TenantTag>())))
                .Concat(TenantTags.Select(tt => new SystemModel(Enumerable.Empty<Role>(), Enumerable.Empty<Tenant>(), Enumerable.Repeat(tt, 1))));
        }
    }
}