using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fastJSON;
using FasTnT.Model;

namespace FasTnT.Formatters.Json
{
    public class JsonRequestFormatter : IRequestFormatter
    {
        public void Write(Request entity, Stream output) => throw new NotImplementedException();

        public Request Read(Stream input)
        {
            using (var reader = new StreamReader(input))
            {
                return ParseInternal(JSON.Parse(reader.ReadToEnd()) as IDictionary<string, object>);
            };
        }

        private Request ParseInternal(IDictionary<string, object> dictionary)
        {
            var body = dictionary["epcisBody"] as IDictionary<string, object>;
            var requestType = (dictionary["epcisBody"] as IDictionary<string, object>).Keys.First();
            var header = new EpcisRequestHeader
            {
                DocumentTime = DateTime.Parse(dictionary["creationDate"].ToString()),
                SchemaVersion = dictionary["schemaVersion"].ToString(),
                RecordTime = DateTime.UtcNow
            };

            switch (body.Keys.First())
            {
                case "vocabularyList":
                    return new EpcisMasterdataDocument { Header = header, MasterDataList = new JsonMasterDataParser().Parse((IList<object>) body.Values.First()) };
                case "eventList":
                    return new EpcisEventDocument { Header = header, EventList = new JsonEventParser().Parse((IList<object>) body.Values.First()).ToArray() };
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
