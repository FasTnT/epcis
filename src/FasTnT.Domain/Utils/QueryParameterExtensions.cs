﻿using FasTnT.Model.Enums;
using FasTnT.Model.Events;
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

            return ChangeType<T>(parameter.Values.Single());
        }

        public static object GetComparisonValue(this QueryParameter parameter)
        {
            if (parameter.Values.Length != 1)
            {
                throw new Exception($"A single value is expected, but multiple were found. Parameter name '{parameter.Name}'");
            }

            return DateTime.TryParse(parameter.Values[0], out DateTime date) ? (object) date : ChangeType<double>(parameter.Values[0]);
        }

        public static T[] GetValues<T>(this QueryParameter parameter)
        {
            return !parameter.Values.Any() ? Array.Empty<T>() : parameter.Values.Select(x => ChangeType<T>(x)).ToArray();
        }

        public static FilterComparator GetComparator(this QueryParameter parameter)
        {
            return Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
        }

        public static SourceDestinationType GetSourceDestinationType(this QueryParameter parameter)
        {
            return parameter.Name.StartsWith("EQ_source") ? SourceDestinationType.Source : SourceDestinationType.Destination;
        }

        public static CustomField GetField(this QueryParameter parameter, FieldType type, bool inner)
        {
            var parts = parameter.Name.Split('_');
            var nameIndex = type == FieldType.CustomField ? inner ? 2 : 1 : inner ? 3 : 2;
            var splittedName = parts[nameIndex].Split('#');

            return new CustomField
            {
                Namespace = splittedName[0],
                Name = splittedName[1],
                Type = type
            };
        }

        public static EpcisField GetAttributeField(this QueryParameter parameter)
        {
            var parts = parameter.Name.Split('_', 3);

            return Enumeration.GetByDisplayName<EpcisField>(parts[1]);
        }

        public static string GetAttributeName(this QueryParameter parameter)
        {
            var parts = parameter.Name.Split('_', 3);

            return parts[2];
        }

        public static EpcType[] GetMatchEpcTypes(this QueryParameter parameter)
        {
            if (!parameter.Name.StartsWith("MATCH_")) throw new Exception("A 'MATCH_*' parameter is expected here.");

            return parameter.Name.Substring(6) switch
            {
                "anyEPC"         => new[] { EpcType.List, EpcType.ChildEpc, EpcType.ParentId, EpcType.InputEpc, EpcType.OutputEpc },
                "epc"            => new[] { EpcType.List, EpcType.ChildEpc },
                "parentID"       => new[] { EpcType.ParentId },
                "inputEPC"       => new[] { EpcType.InputEpc },
                "outputEPC"      => new[] { EpcType.OutputEpc },
                "epcClass"       => new[] { EpcType.Quantity, EpcType.ChildQuantity },
                "inputEpcClass"  => new[] { EpcType.InputQuantity },
                "outputEpcClass" => new[] { EpcType.OutputQuantity },
                "anyEpcClass"    => new[] { EpcType.Quantity, EpcType.InputQuantity, EpcType.OutputQuantity },
                _                => throw new Exception($"Unknown 'MATCH_*' parameter: '{parameter.Name}'")
            };
        }

        private static T ChangeType<T>(string value)
        {
            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(value, typeof(T));
            else if (typeof(T) == typeof(DateTime))
                return (T)Convert.ChangeType(DateTime.Parse(value), typeof(T));
            else if (typeof(T) == typeof(int))
                return (T)Convert.ChangeType(int.Parse(value), typeof(T));
            else if (typeof(T) == typeof(double))
                return (T)Convert.ChangeType(double.Parse(value), typeof(T));
            else if (typeof(T) == typeof(bool))
                return (T)Convert.ChangeType(bool.Parse(value), typeof(T));
            else
                throw new Exception($"Invalid value type: '{typeof(T).Name}'");
        }
    }
}
