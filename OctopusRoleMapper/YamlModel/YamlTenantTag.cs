using System;
using System.ComponentModel;
using System.Linq;
using OctopusRoleMapper.Model;
using YamlDotNet.Serialization;

namespace OctopusRoleMapper.YamlModel
{
    [Description("Inverted octopus TenantTag model")]
    [Serializable]
    public class YamlTenantTag
    {
        [YamlMember(Order = 1)]
        public string Name { get; set; }

        [YamlMember(Order = 2)]
        public string[] Machines { get; set; }

        public TenantTag ToModel()
        {
            return new TenantTag(Name, Machines);
        }

        public static YamlTenantTag FromModel(TenantTag model)
        {
            return new YamlTenantTag
            {
                Name = model.Name,
                Machines = model.Machines.ToArray()
            };
        }
    }
}
