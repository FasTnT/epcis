using FasTnT.Commands.Responses;
using FasTnT.Parsers.Xml.Formatters;
using FasTnT.UnitTest;
using System.IO;
using System.Threading.Tasks;

namespace FasTnT.IntegrationTests.Formatters.XML
{
    public abstract class XmlFormatterTestBase : TestBase
    {
        public XmlResponseFormatter Formatter { get; set; }
        public IEpcisResponse Response { get; set; }
        public string Formatted { get; set; }

        public override void Given()
        {
            Formatter = new XmlResponseFormatter();
        }

        public override void When()
        {
            using(var stream = new MemoryStream())
            {
                Task.WaitAll(Formatter.Write(Response, stream, default));
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    Formatted = reader.ReadToEnd();
                }
            }
        }
    }
}
