using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OctopusRoleMapper.Tests.Helpers;
using Ploeh.AutoFixture;

namespace OctopusRoleMapper.Tests
{
    [TestFixture]
    public class YamlModelWriterTests
    {
        private YamlModelWriter _writer;

        [SetUp]
        public void SetUp()
        {
            _writer = new YamlModelWriter();
        }

        [Test]
        public void It_should_write_all_data()
        {
            var expected = new Fixture().Create<YamlRole>();
            var content = Write(expected);

            var actual = new YamlModelReader().Read(new MemoryStream(Encoding.UTF8.GetBytes(content))).Single();
            actual.AssertDeepEqualsTo(expected);
        }


        private string Write(YamlRole model)
        {
            using (var stream = new MemoryStream())
            {
                _writer.Write(stream, model);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
