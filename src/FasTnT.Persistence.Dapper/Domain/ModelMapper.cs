using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Subscriptions;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FasTnT.Persistence.Dapper
{
    public static class ModelMapper
    {
        static IDictionary<Type, PropertyInfo[]> properties = new Dictionary<Type, PropertyInfo[]>();
        static Type[] types = new[] { typeof(Epc), typeof(EpcisEvent), typeof(ErrorDeclaration), typeof(SourceDestination), typeof(BusinessTransaction), typeof(CorrectiveEventId), typeof(CustomField), typeof(EpcisRequestHeader), typeof(MasterDataField), typeof(Subscription) };

        static ModelMapper() => types.ForEach(t => properties.Add(t, t.GetProperties().ToArray()));

        public static T Map<TU, T>(this TU source, Action<T> propertySetting = null) where T : TU, new()
        {
            if (!properties.TryGetValue(typeof(TU), out var tprops)) throw new Exception($"Unknown type to be mapped: '{typeof(TU).Name}'");

            var target = new T();
            tprops.ForEach(prop =>
            {
                var sp = typeof(TU).GetProperty(prop.Name);
                if (sp != null)
                {
                    var value = sp.GetValue(source, null);
                    typeof(T).GetProperty(prop.Name).SetValue(target, value, null);
                }
            });

            propertySetting?.Invoke(target);

            return target;
        }
    }
}
