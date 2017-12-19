using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using OctopusRoleMapper.Helpers;
using OctopusRoleMapper.Model;
using OctopusRoleMapper.YamlModel;

namespace OctopusRoleMapper
{
    public class YamlRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger<YamlRepository>();

        private readonly YamlModelReader _reader = new YamlModelReader();
        private readonly YamlModelWriter _writer = new YamlModelWriter();

        public void Save(SystemModel model, string modelDirectory)
        {
            Directory.CreateDirectory(modelDirectory);

            foreach (var splitModel in model.SplitModel().Select(YamlSystemModel.FromModel))
                SaveModel(splitModel, GetModelPath(splitModel, modelDirectory));
        }

        private void SaveModel(YamlSystemModel model, string path)
        {
            Logger.Info($"Saving: {Path.GetFileName(path)}");
            WriteFile(path, model);
        }

        private void WriteFile(string file, params YamlSystemModel[] models)
        {
            using (var stream = new FileStream(file, FileMode.Create))
                _writer.Write(stream, models);
        }

        public SystemModel Load(string modelDirectory)
        {
            var model = new YamlSystemModel();
            var files = FindFiles(modelDirectory);
            foreach (var subModel in files.SelectMany(LoadModels))
                model.MergeIn(subModel);
            return model.BuildWith(new SystemModelBuilder()).Build();
        }

        private YamlSystemModel[] LoadModels(string path)
        {
            Logger.Info($"Loading: {Path.GetFileName(path)}");
            return ReadFile(path);
        }

        private YamlSystemModel[] ReadFile(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
                return _reader.Read(stream);
        }

        private string GetModelPath(YamlSystemModel splitModel, string modelDirectory)
        {
            var name = splitModel.Roles.EnsureNotNull().Select(x => $"Role_{x.Name.SanitiseNameIfNeeded()}.yml")
                .Concat(splitModel.Tenants.EnsureNotNull().Select(x => $"Tenant_{x.Name.SanitiseNameIfNeeded()}.yml"))
                .Concat(splitModel.TenantTags.EnsureNotNull().Select(x => $"TenantTag_{x.Name.SanitiseNameIfNeeded()}.yml"))
                .Single();
            return modelDirectory + "\\" + name;
        }

        private static IEnumerable<string> FindFiles(string modelDirectory)
        {
            return Directory.EnumerateFiles(modelDirectory, "*.yml", SearchOption.AllDirectories);
        }
    }
}
