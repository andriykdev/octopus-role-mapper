using System.Collections.Generic;
using System.Linq;

namespace OctopusRoleMapper.Model
{
    public class TenantTag
    {
        public string Name { get; }

        public IEnumerable<string> Machines { get; }

        public TenantTag(string name, IEnumerable<string> machines)
        {
            Name = name;
            Machines = machines?.OrderBy(x => x).ToArray() ?? new string[] { };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}