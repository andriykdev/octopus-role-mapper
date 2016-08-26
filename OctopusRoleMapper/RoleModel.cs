using System.Collections.Generic;
using System.Linq;

namespace OctopusRoleMapper
{
    public class RoleModel
    {
        public IEnumerable<Role> Roles { get; }

        public RoleModel(IEnumerable<Role> roles)
        {
            Roles = roles.OrderBy(x => x.Name).ToArray();
        }
    }
}
