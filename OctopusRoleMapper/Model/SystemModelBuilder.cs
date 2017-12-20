using System.Collections.Generic;

namespace OctopusRoleMapper.Model
{
    public class SystemModelBuilder
    {
        private readonly List<Role> _roles = new List<Role>();
        private readonly List<Tenant> _tenants = new List<Tenant>();
        private readonly List<TenantTag> _tenantTags = new List<TenantTag>();

        public SystemModelBuilder AddRole(Role role)
        {
            _roles.Add(role);
            return this;
        }

        public SystemModelBuilder AddTenant(Tenant tenant)
        {
            _tenants.Add(tenant);
            return this;
        }

        public SystemModelBuilder AddTenantTag(TenantTag tag)
        {
            _tenantTags.Add(tag);
            return this;
        }

        public SystemModel Build()
        {
            return new SystemModel(_roles, _tenants, _tenantTags);
        }
    }
}
