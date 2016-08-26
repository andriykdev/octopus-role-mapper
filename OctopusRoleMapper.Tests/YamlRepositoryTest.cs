using System;
using System.IO;
using NUnit.Framework;
using OctopusRoleMapper.Tests.Helpers;
using Ploeh.AutoFixture;

namespace OctopusRoleMapper.Tests
{
    [TestFixture]
    public class YamlRepositoryTest
    {
        private string _directory;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _directory = Guid.NewGuid().ToString();
            Directory.CreateDirectory(_directory);

            _fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_directory, true);
        }

        [Test]
        public void Repository_should_save_and_load_model()
        {
            var repository = new YamlRepository();

            var expected = _fixture.Create<RoleModel>();

            repository.Save(expected, _directory);

            var actual = repository.Load(_directory);

            actual.AssertDeepEqualsTo(expected);
        }
    }
}
