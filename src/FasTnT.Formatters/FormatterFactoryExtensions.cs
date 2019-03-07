using System;
using System.Collections.Generic;
using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using ModelDictionary = System.Collections.Generic.Dictionary<System.Type, System.Func<FasTnT.Formatters.IFormatterFactory, object>>;

namespace FasTnT.Formatters
{
    public static class FormatterFactoryExtensions
    {
        private static ModelDictionary _formatters = new ModelDictionary
        {
            { typeof(Request), factory => factory.RequestFormatter },
            { typeof(EpcisQuery), factory => factory.QueryFormatter },
            { typeof(IEpcisResponse), factory => factory.ResponseFormatter }
        };

        public static IFormatter<T> GetFormatter<T>(this IFormatterFactory factory)
        {
            return _formatters.ContainsKey(typeof(T)) 
                ? _formatters[typeof(T)](factory) as IFormatter<T>
                : throw new ArgumentException($"Unable to find a formatter factory for type '{typeof(T).Name}'");
        }
    }
}
