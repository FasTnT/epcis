using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml.Parsers.Query;
using System.IO;

namespace FasTnT.UnitTest.Parsers.Soap
{
    public abstract class SoapParserTestBase : TestBase
    {
        public MemoryStream PollStream { get; set; }
        public IQueryRequest Result { get; set; }

        public override void When()
        {
            Result = new SoapQueryParser().Read(PollStream, default).Result;
        }

        public void SetRequest(string request)
        {
            PollStream = new MemoryStream();
            var sw = new StreamWriter(PollStream);
            sw.WriteLine(request);
            sw.Flush();
            PollStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
