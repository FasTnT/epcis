using System.Collections.Generic;
using System.Linq;
using FasTnT.Model.MasterDatas;

namespace FasTnT.Formatters.Json
{
    internal class JsonMasterDataParser
    {
        public IList<EpcisMasterData> Parse(IList<object> masterdata)
        {
            return masterdata.SelectMany(ParseMasterdata).ToList();
        }

        private IList<EpcisMasterData> ParseMasterdata(object masterdataType)
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
                Attributes = ParseAttributes(masterData["attributes"] as IList<object>, type, masterData["id"].ToString()).ToList()
            };
        }

        private IEnumerable<MasterDataAttribute> ParseAttributes(IList<object> masterDataAttributes, string type, string id)
        {
            foreach(var attr in masterDataAttributes.Cast<IDictionary<string, object>>())
            {
                yield return new MasterDataAttribute
                {
                    Id = attr["id"].ToString(),
                    ParentType = type,
                    ParentId = id,
                    Value = attr.ContainsKey("attribute") ? attr["attribute"]?.ToString() : null
                };
            }
        }
    }
}
