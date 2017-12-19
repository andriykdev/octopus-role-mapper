using System;
using System.ComponentModel;
using System.Linq;
using OctopusRoleMapper.Model;
using YamlDotNet.Serialization;

namespace OctopusRoleMapper.YamlModel
{
    [Description("Inverted octopus tenant model")]
    [Serializable]
    public class YamlTenant
    {
        [YamlMember(Order = 1)]
        public string Name { get; set; }

        [YamlMember(Order = 2)]
        public string[] Machines { get; set; }

        public Tenant ToModel()
        {
            return new Tenant(Name, Machines);
        }

        public static YamlTenant FromModel(Tenant model)
        {
            return new YamlTenant
            {
                Name = model.Name,
                Machines = model.Machines.ToArray()
            };
        }
    }
}
