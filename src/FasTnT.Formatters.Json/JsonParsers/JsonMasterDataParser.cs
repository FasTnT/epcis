using System.Collections.Generic;
using System.Linq;
using FasTnT.Model.MasterDatas;
using Newtonsoft.Json.Linq;

namespace FasTnT.Formatters.Json
{
    internal class JsonMasterDataParser
    {
        public IList<EpcisMasterData> Parse(IEnumerable<JObject> masterdata)
        {
            return masterdata.SelectMany(ParseMasterdata).ToList();
        }

        private IList<EpcisMasterData> ParseMasterdata(JObject masterdataType)
        {
            var dict = masterdataType as IDictionary<string, object>;
            var type = dict["type"].ToString();

            return (dict["vocabularyElementList"] as List<object>).Select(x => ParseMasterdata(type, (IDictionary<string, object>)x)).ToList();
        }

        private EpcisMasterData ParseMasterdata(string type, IDictionary<string, object> masterData)
        {
            return new EpcisMasterData
            {
                Type = type,
                Id = masterData["id"].ToString(),
                Attributes = (masterData["attributes"] as IList<object>).Cast<IDictionary<string, object>>().Select(x => ParseAttributes(x, type, masterData["id"].ToString())).ToList()
            };
        }

        private MasterDataAttribute ParseAttributes(IDictionary<string, object> masterDataAttributes, string type, string id)
        {
            return new MasterDataAttribute
            {
                Id = masterDataAttributes["id"].ToString(),
                ParentType = type,
                ParentId = id,
                Value = masterDataAttributes.ContainsKey("attribute") ? masterDataAttributes["attribute"]?.ToString() : null
            };
        }
    }
}
