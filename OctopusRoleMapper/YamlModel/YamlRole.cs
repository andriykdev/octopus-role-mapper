using System;
using System.ComponentModel;
using System.Linq;
using OctopusRoleMapper.Model;
using YamlDotNet.Serialization;

namespace OctopusRoleMapper.YamlModel
{
    [Description("Inverted octopus role model")]
    [Serializable]
    public class YamlRole
    {
        [YamlMember(Order = 1)]
        public string Name { get; set; }

        [YamlMember(Order = 2)]
        public string[] Machines { get; set; }

        public Role ToModel()
        {
            return new Role(Name, Machines);
        }

        public static YamlRole FromModel(Role model)
        {
            return new YamlRole
            {
                Name = model.Name,
                Machines = model.Machines.ToArray()
            };
        }
    }
}
