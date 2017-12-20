using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OctopusRoleMapper.Tests.Helpers;
using OctopusRoleMapper.YamlModel;

namespace OctopusRoleMapper.Tests
{
    [TestFixture]
    public class YamlModelReaderTests
    {
        private YamlModelReader _reader;

        [SetUp]
        public void SetUp()
        {
            _reader = new YamlModelReader();
        }

        [Test]
        public void It_Should_Read_Role()
        {
            var content = @"---
Roles:
- Name: api
  Machines:
  - Dev2
  - Dev3
  - newbox
...
";


            var role = new YamlRole
            {
                Name = "api",
                Machines = new[] { "Dev2", "Dev3", "newbox" }
            };

            var expected = new YamlSystemModel() { Roles = new[] { role } };

            var model = Read(content);
            model.AssertDeepEqualsTo(expected);
        }

        [Test]
        public void It_Should_Read_Role_With_No_Mapped_Machines()
        {
            var content = @"---
Roles:
- Name: api
  Machines:
...
";

            var model = Read(content);
            Assert.That(model.Roles[0].Name.Equals("api"));
            Assert.IsNull(model.Roles[0].Machines);

        }

        private YamlSystemModel Read(string content)
        {
            return _reader.Read(new MemoryStream(Encoding.UTF8.GetBytes(content), false)).Single();
        }
    }
}
