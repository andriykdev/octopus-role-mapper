using System.IO;
using YamlDotNet.Serialization;

namespace OctopusRoleMapper
{
    public class YamlModelWriter
    {
        private readonly Serializer _serializer = new Serializer();

        public void Write(Stream stream, YamlRole model)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("---");
                _serializer.Serialize(writer, model);
                writer.WriteLine("...");
            }
        }
    }
}
