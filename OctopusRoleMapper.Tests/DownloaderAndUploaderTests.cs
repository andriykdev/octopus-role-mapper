using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Octopus.Client.Model;
using OctopusRoleMapper.Tests.Helpers;

namespace OctopusRoleMapper.Tests
{
    [TestFixture]
    public class DownloaderAndUploaderTests
    {
        private Downloader _downloader;
        private FakeOctopusRepository _repository;
        private Uploader _uploader;
        private RoleModel _model;

        [SetUp]
        public void SetUp()
        {
            _repository = new FakeOctopusRepository();
            _downloader = new Downloader(_repository);
            _uploader = new Uploader(_repository);

            _model = new RoleModel(new[]
            {
                new Role("api", new List<string> {"dev1", "dev2"}),
                new Role("csapi", new List<string> {"dev1", "dev2"}),
                new Role("service", new List<string> {"dev3"})
            });

            foreach (var role in _model.Roles)
            {
                _repository.FakeMachineRoles.Add(role.Name);
            }

            foreach (var machineName in _model.Roles.SelectMany(x => x.Machines).Distinct())
            {
                var machine = new MachineResource { Name = machineName };

                foreach (var machineRole in _model.Roles.Where(x => x.Machines.Contains(machineName)).Select(r => r.Name).ToList())
                {
                    machine.Roles.Add(machineRole);
                }
                _repository.Machines.Create(machine);
            }
        }

        [Test]
        public void Uploader_should_not_be_case_sensitive()
        {
            var caseSensitiveModel = new RoleModel(new[]
            {
                new Role("api", new List<string> {"DEV1", "Dev2"}),
                new Role("csapi", new List<string> {"dev1", "dev2"}),
                new Role("service", new List<string> {"dev3"})
            });

            _uploader.UploadModel(caseSensitiveModel);
            var actual = _downloader.DownloadModel();
            actual.AssertDeepEqualsTo(_model);
        }

        [Test]
        public void It_should_download_model()
        {
            var actual = _downloader.DownloadModel();

            actual.AssertDeepEqualsTo(_model);
        }

        [Test]
        public void It_should_upload_model()
        {
            _uploader.UploadModel(_model);

            var actual = _downloader.DownloadModel();

            actual.AssertDeepEqualsTo(_model);
        }

        [Test]
        public void It_should_Upload_model_and_do_not_touch_unknow_roles()
        {
            const string unknownRole = "unknownrole";
            const string machineName = "dev3";

            _repository.FakeMachineRoles.Add(unknownRole);

            var machine = _repository.Machines.FindByName(machineName);
            machine.Roles.Add(unknownRole);
            _repository.Machines.Modify(machine);

            _uploader.UploadModel(_model);
            var actual = _downloader.DownloadModel();

            Assert.That(actual.Roles.Select(x => x.Name).Contains(unknownRole));
            Assert.That(actual.Roles.Single(x => x.Name.Equals(unknownRole)).Machines.Contains(machineName));
        }

        [Test]
        public void It_should_move_known_role()
        {
            _model = new RoleModel(new[]
            {
                new Role("api", new List<string> {"dev3", "dev2"}),
            });

            _uploader.UploadModel(_model);
            var actual = _downloader.DownloadModel();

            Assert.That(actual.Roles.Single(x => x.Name == "api").Machines.Contains("dev3"));
            Assert.That(actual.Roles.Single(x => x.Name == "api").Machines.Contains("dev2"));
            Assert.That(!actual.Roles.Single(x => x.Name == "api").Machines.Contains("dev1"));
        }

        [Test]
        public void It_should_throw_if_machine_is_missing_on_octopus()
        {
            const string missingMachine = "dev3";
            _repository.Machines.Delete(_repository.Machines.FindByName(missingMachine));

            var ex = Assert.Throws<InvalidOperationException>(() => _uploader.UploadModel(_model));

            Assert.That(ex.Message, Is.EqualTo("Local mappped machine(s) are missing on Octopus"));
        }

        [Test]
        public void It_should_throw_if_there_is_zero_roles_for_machine()
        {
            _model = new RoleModel(new[]
            {
                new Role("service", new List<string> {"dev1"})
            });

            var ex = Assert.Throws<InvalidOperationException>(() => _uploader.UploadModel(_model));

            Assert.That(ex.Message, Is.EqualTo("Machine dev3 has no roles"));
        }
    }
}