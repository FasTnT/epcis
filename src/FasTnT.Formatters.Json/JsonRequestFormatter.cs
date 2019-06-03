using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fastJSON;
using FasTnT.Model;

namespace FasTnT.Formatters.Json
{
    public class JsonRequestFormatter
    {
        public Task Write(Request entity, Stream output, CancellationToken cancellationToken) => throw new NotImplementedException();

        public async Task<Request> Read(Stream input, CancellationToken cancellationToken)
        {
            using (var reader = new StreamReader(input))
            {
                return ParseInternal(JSON.Parse(await reader.ReadToEndAsync()) as IDictionary<string, object>);
            };
        }

        private Request ParseInternal(IDictionary<string, object> dictionary)
        {
            var body = dictionary["epcisBody"] as IDictionary<string, object>;
            var header = new EpcisRequestHeader
            {
                DocumentTime = DateTime.Parse(dictionary["creationDate"].ToString()),
                SchemaVersion = dictionary["schemaVersion"].ToString(),
                RecordTime = DateTime.UtcNow
            };

            switch (body.Keys.First())
            {
                case "vocabularyList":
                    return new CaptureRequest { Header = header, MasterDataList = new JsonMasterDataParser().Parse((IList<object>) body.Values.First()) };
                case "eventList":
                    return new CaptureRequest { Header = header, EventList = new JsonEventParser().Parse((IList<object>) body.Values.First()).ToArray() };
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
