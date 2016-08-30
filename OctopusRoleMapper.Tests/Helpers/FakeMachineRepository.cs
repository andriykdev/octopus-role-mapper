using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace OctopusRoleMapper.Tests.Helpers
{
    internal class FakeMachineRepository : FakeRepository<MachineResource>, IMachineRepository
    {
        public MachineResource FindByName(string name)
        {
            return FindOne(t => t.Name == name);
        }

        public List<MachineResource> FindByNames(IEnumerable<string> names)
        {
            return FindMany(t => names.Contains(t.Name));
        }

        public MachineResource Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null)
        {
            throw new NotImplementedException();
        }

        public MachineConnectionStatus GetConnectionStatus(MachineResource machine)
        {
            throw new NotImplementedException();
        }

        public List<MachineResource> FindByThumbprint(string thumbprint)
        {
            throw new NotImplementedException();
        }
    }
}