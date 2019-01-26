using FasTnT.Model.Events.Enums;
using System;

namespace FasTnT.Model.Queries.Implementations
{
    public static class SimpleEventQueryParameterExtensions
    {
        public static EpcType[] GetMatchEpcTypes(this QueryParameter parameter)
        {
            if (!parameter.Name.StartsWith("MATCH_")) throw new Exception("A 'MATCH_*' parameter is expected here.");

            switch (parameter.Name.Substring(6))
            {
                case "anyEPC": return new[] { EpcType.List, EpcType.ChildEpc, EpcType.ParentId, EpcType.InputEpc, EpcType.OutputEpc };
                case "epc": return new[] { EpcType.List, EpcType.ChildEpc };
                case "parentID": return new[] { EpcType.ParentId };
                case "inputEPC": return new[] { EpcType.InputEpc };
                case "outputEPC": return new[] { EpcType.OutputEpc };
                case "epcClass": return new[] { EpcType.Quantity };
                case "inputEpcClass": return new[] { EpcType.InputQuantity };
                case "outputEpcClass": return new[] { EpcType.OutputQuantity };
                case "anyEpcClass": return new[] { EpcType.Quantity, EpcType.InputQuantity, EpcType.OutputQuantity };
            }

            throw new Exception($"Unknown 'MATCH_*' parameter: '{parameter.Name}'");
        }
    }
}
