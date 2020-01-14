using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Queries;
using FasTnT.Model.Utils;
using System;
using System.Linq;

namespace FasTnT.Domain.Utils
{
    public static class QueryParameterExtensions
    {
        public static T GetValue<T>(this QueryParameter parameter)
        {
            if (parameter.Values.Length != 1)
            {
                throw new Exception($"A single value is expected, but multiple were found. Parameter name '{parameter.Name}'");
            }

            if (typeof(T) == typeof(DateTime))
                return (T)Convert.ChangeType(DateTime.Parse(parameter.Values.Single()), typeof(T));
            else if (typeof(T) == typeof(int))
                return (T)Convert.ChangeType(int.Parse(parameter.Values.Single()), typeof(T));
            else if (typeof(T) == typeof(double))
                return (T)Convert.ChangeType(double.Parse(parameter.Values.Single()), typeof(T));
            else if (typeof(T) == typeof(bool))
                return (T)Convert.ChangeType(bool.Parse(parameter.Values.Single()), typeof(T));
            else
                throw new Exception($"Unknow value type: '{typeof(T).Name}'");
        }

        public static FilterComparator GetComparator(this QueryParameter parameter)
        {
            return Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
        }

        public static CustomField GetField(this QueryParameter parameter, FieldType type, bool inner)
        {
            var parts = parameter.Name.Split('_');
            var nameIndex = type == FieldType.EventExtension ? inner ? 2 : 1 : inner ? 3 : 2;
            var splittedName = parts[nameIndex].Split('#');

            return new CustomField
            {
                Name = splittedName[0],
                Namespace = splittedName[1],
                Type = type
            };
        }

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
                case "epcClass": return new[] { EpcType.Quantity, EpcType.ChildQuantity };
                case "inputEpcClass": return new[] { EpcType.InputQuantity };
                case "outputEpcClass": return new[] { EpcType.OutputQuantity };
                case "anyEpcClass": return new[] { EpcType.Quantity, EpcType.InputQuantity, EpcType.OutputQuantity };
            }

            throw new Exception($"Unknown 'MATCH_*' parameter: '{parameter.Name}'");
        }
    }
}
