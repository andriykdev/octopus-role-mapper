using System;
using System.ComponentModel;
using System.Linq;
using OctopusRoleMapper.Helpers;
using OctopusRoleMapper.Model;

namespace OctopusRoleMapper.YamlModel
{
    [Serializable]
    [Description("Octopus model root type.")]
    public class YamlSystemModel
    {
        public YamlSystemModel()
        {
        }

        private YamlSystemModel(YamlRole[] roles, YamlTenant[] tenants, YamlTenantTag[] tags)
        {
            Roles = roles;
            Tenants = tenants;
            TenantTags = tags;
        }

        [Description("List of Machine Roles.")]
        public YamlRole[] Roles { get; set; }

        [Description("List of Machine Tenants.")]
        public YamlTenant[] Tenants { get; set; }

        [Description("List of Machine TenantTags.")]
        public YamlTenantTag[] TenantTags { get; set; }

        public SystemModelBuilder BuildWith(SystemModelBuilder builder)
        {
            foreach (var role in Roles.EnsureNotNull())
                builder.AddRole(role.ToModel());

            foreach (var tenant in Tenants.EnsureNotNull())
                builder.AddTenant(tenant.ToModel());

            foreach (var tag in TenantTags.EnsureNotNull())
                builder.AddTenantTag(tag.ToModel());

            return builder;
        }

        public static YamlSystemModel FromModel(SystemModel model)
        {
            return new YamlSystemModel(
                model.Roles.Select(YamlRole.FromModel).ToArray().NullIfEmpty(),
                model.Tenants.Select(YamlTenant.FromModel).ToArray().NullIfEmpty(),
                model.TenantTags.Select(YamlTenantTag.FromModel).ToArray().NullIfEmpty());
        }

        public YamlSystemModel MergeIn(YamlSystemModel model)
        {
            Roles = this.MergeItemsIn(model, x => x.Roles);
            Tenants = this.MergeItemsIn(model, x => x.Tenants);
            TenantTags = this.MergeItemsIn(model, x => x.TenantTags);
            return this;
        }
    }
}
