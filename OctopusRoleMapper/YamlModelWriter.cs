using System.IO;
using OctopusRoleMapper.YamlModel;
using YamlDotNet.Serialization;

namespace OctopusRoleMapper
{
    public class YamlModelWriter
    {
        private readonly Serializer _serializer = new Serializer();

        public void Write(Stream stream,params YamlSystemModel[] models)
        {
            using (var writer = new StreamWriter(stream))
            {
                foreach (var model in models)
                {
                    writer.WriteLine("---");
                    _serializer.Serialize(writer, model);
                }
                writer.WriteLine("...");
            }
        }
    }
}