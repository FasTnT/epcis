using FasTnT.Model.Queries;
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
    }
}
