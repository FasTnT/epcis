using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Formatters.Json.Utils;
using FasTnT.Model;
using Newtonsoft.Json.Linq;

namespace FasTnT.Formatters.Json
{
    public class JsonRequestFormatter
    {
        public async Task<Request> Read(Stream input)
        {
            var document = await JsonValidator.Instance.Load(input);
            return ParseInternal(document);
        }

        private Request ParseInternal(IDictionary<string, JToken> dictionary)
        {
            var body = dictionary["epcisBody"].ToObject<Dictionary<string, JArray>>();
            var header = new EpcisRequestHeader
            {
                DocumentTime = DateTime.Parse(dictionary["creationDate"].ToString()),
                SchemaVersion = dictionary["schemaVersion"].ToString(),
                RecordTime = DateTime.UtcNow
            };

            switch (body.Keys.First())
            {
                case "vocabularyList":
                    return new CaptureRequest { Header = header, MasterDataList = new JsonMasterDataParser().Parse(body.Values.First().Cast<JObject>()) };
                case "eventList":
                    return new CaptureRequest { Header = header, EventList = new JsonEventParser().Parse(body.Values.First().Cast<JObject>()).ToArray() };
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
