using FasTnT.Domain;
using FasTnT.Model.Utils;
using System;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class MatchEpcParameter : SimpleEventQueryParameter
    {
        public EpcType Type
        {
            get
            {
                var extractedType = Name.Substring(6);

                if(extractedType.EndsWith("Class", StringComparison.OrdinalIgnoreCase))
                {
                    extractedType = extractedType.Substring(0, extractedType.Length - 5);
                }

                return Enumeration.GetByDisplayName<EpcType>(extractedType);
            }
        }
    }
}
