using System;
using System.IO;
using NUnit.Framework;
using OctopusRoleMapper.Model;
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

            var expected = _fixture.Create<SystemModel>();

            repository.Save(expected, _directory);

            var actual = repository.Load(_directory);

            actual.AssertDeepEqualsTo(expected);
        }

        [Test] public void Repository_should_load_roles_with_no_mapped_machines()
        {
            var model = new SystemModel(new[]
            {
                new Role("api", new [] {"dev1", "dev2"}),
                new Role("csapi", new string[] {}),
                new Role("service", null)
            }, new[]
            {
                new Tenant("a", new []{ "dev1"}),
            }, new[]
            {
                new TenantTag("a", new [] { "dev1"})
            });

            var repository = new YamlRepository();

            repository.Save(model, _directory);

            var actual = repository.Load(_directory);

            actual.AssertDeepEqualsTo(model);
        }
    }
}
