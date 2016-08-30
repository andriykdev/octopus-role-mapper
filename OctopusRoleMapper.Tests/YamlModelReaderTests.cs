using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OctopusRoleMapper.Tests.Helpers;

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
Name: api
Machines:
- Dev2
- Dev3
- newbox
...
";

            var expected = new YamlRole
            {
                Name = "api",
                Machines = new [] { "Dev2", "Dev3", "newbox" }
            };

            var model = Read(content);
            model.AssertDeepEqualsTo(expected);
        }

        [Test]
        public void It_Should_Read_Role_With_No_Mapped_Machines()
        {
            var content = @"---
Name: api
Machines:
...
";

            var model = Read(content);
            Assert.That(model.Name.Equals("api"));
            Assert.IsNull(model.Machines);

        }

        private YamlRole Read(string content)
        {
            return _reader.Read(new MemoryStream(Encoding.UTF8.GetBytes(content), false)).Single();
        }
    }
}
