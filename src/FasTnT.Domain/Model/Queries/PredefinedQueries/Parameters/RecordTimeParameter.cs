using FasTnT.Domain.Utils;
using System;

namespace FasTnT.Domain.Model.Queries.PredefinedQueries.Parameters
{
    public class RecordTimeParameter : SimpleEventQueryParameter
    {
        public ParameterComparator Comparator => Enumeration.GetByDisplayName<ParameterComparator>(Name.Substring(0, 2));
        public DateTime DateValue => DateTime.Parse(Value);
    }
}
