using System;
using System.Collections.Generic;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public abstract class SimpleEventQueryParameter
    {
        public string Name { get; set; }
        public IEnumerable<string> Values { get; set; }
        public string Value { get { return Values?.SingleWhenOnly(); } }
        public ParameterValueType ValueType => ParameterValueType.Parse(Values);
        public DateTime? DateValue => (ValueType == ParameterValueType.Date) ? DateTime.Parse(Value) : default(DateTime?);
        public double? NumericValue => (ValueType == ParameterValueType.Numeric) ? double.Parse(Value) : default(double?);
    }
}
