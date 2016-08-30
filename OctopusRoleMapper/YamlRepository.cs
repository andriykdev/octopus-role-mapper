using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;

namespace OctopusRoleMapper
{
    public class YamlRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger<YamlRepository>();

        private readonly YamlModelReader _reader = new YamlModelReader();
        private readonly YamlModelWriter _writer = new YamlModelWriter();

        public void Save(RoleModel model, string modelDirectory)
        {
            Directory.CreateDirectory(modelDirectory);

            foreach (var role in model.Roles)
            {
                string path = modelDirectory + "\\" + $"Role_{role.Name}.yml";
                Logger.Info($"Saving role: {role.Name}");
                WriteFile(path, YamlRole.FromModel(role));
            }
        }

        private void WriteFile(string file, YamlRole model)
        {
            using (var stream = new FileStream(file, FileMode.Create))
                _writer.Write(stream, model);
        }

        public RoleModel Load(string modelDirectory)
        {
            Logger.Info($"Loading roles from {modelDirectory}");
            var files = FindFiles(modelDirectory);
            return new RoleModel(files.SelectMany(LoadModels).Select(m => m.ToModel()));
        }

        private YamlRole[] LoadModels(string path)
        {
            Logger.Info($"Loading: {Path.GetFileName(path)}");

            using (var stream = new FileStream(path, FileMode.Open))
                return _reader.Read(stream);
        }

        private static IEnumerable<string> FindFiles(string modelDirectory)
        {
            return Directory.EnumerateFiles(modelDirectory, "*.yml", SearchOption.AllDirectories);
        }
    }
}
