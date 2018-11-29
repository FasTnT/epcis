using FasTnT.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class ParameterValueType : Enumeration
    {
        public static ParameterValueType Numeric = new ParameterValueType(0, "Numeric");
        public static ParameterValueType Date = new ParameterValueType(1, "Date");
        public static ParameterValueType Text = new ParameterValueType(2, "Text");

        public ParameterValueType() { }
        public ParameterValueType(short id, string displayName) : base(id, displayName) { }

        public static ParameterValueType Parse(IEnumerable<string> values)
        {
            if (values.Count() != 1) return Text; // List of values can only be for Text

            var value = values.Single();

            if (DateTime.TryParse(value, out DateTime dateValue)) return Date;
            if (double.TryParse(value, out double numericValue)) return Numeric;

            return Text;
        }
    }
}
