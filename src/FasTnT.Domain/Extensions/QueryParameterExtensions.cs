using FasTnT.Model.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Model.Extensions
{
    public static class QueryParameterExtensions
    {
        public static T GetValue<T>(this QueryParameter parameter)
        {
            if(parameter.Values.Length != 1)
            {
                throw new Exception($"A single value is expected, but multiple were found. Parameter name '{parameter.Name}'");
            }

            if(typeof(T) == typeof(DateTime))
            {
                return (T) Convert.ChangeType(DateTime.Parse(parameter.Values.Single()), typeof(T));
            }
            if (typeof(T) == typeof(int))
            {
                return (T)Convert.ChangeType(int.Parse(parameter.Values.Single()), typeof(T));
            }
            if (typeof(T) == typeof(double))
            {
                return (T)Convert.ChangeType(double.Parse(parameter.Values.Single()), typeof(T));
            }

            throw new Exception($"Unknow value type: '{typeof(T).Name}'");
        }

        public static IEnumerable<T> GetValues<T>(this QueryParameter parameter)
        {
            if (typeof(T) == typeof(DateTime))
            {
                return parameter.Values.Select(DateTime.Parse).Cast<T>();
            }
            if (typeof(T) == typeof(int))
            {
                return parameter.Values.Select(int.Parse).Cast<T>();
            }
            if (typeof(T) == typeof(double))
            {
                return parameter.Values.Select(double.Parse).Cast<T>();
            }

            throw new Exception($"Unknow value type: '{typeof(T).Name}'");
        }
    }
}
