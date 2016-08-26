using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OctopusRoleMapper
{
    public class YamlModelReader
    {
        private readonly Deserializer _deserializer = new Deserializer();

        public YamlRole[] Read(Stream stream)
        {
            var models = new List<YamlRole>();
            using (var reader = new StreamReader(stream))
            {
                var eventReader = new EventReader(new Parser(reader));
                eventReader.Expect<StreamStart>();

                while (eventReader.Accept<DocumentStart>())
                    models.Add(_deserializer.Deserialize<YamlRole>(eventReader));

                return models.ToArray();
            }
        }
    }
}
