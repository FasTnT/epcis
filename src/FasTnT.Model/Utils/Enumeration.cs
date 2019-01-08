using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FasTnT.Model.Utils
{
    public abstract class Enumeration : IComparable
    {
        public string DisplayName { get; private set; }
        public short Id { get; private set; }

        protected Enumeration()
        {
        }

        protected Enumeration(short id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
        {
            var type = typeof(T);
            var fields = type.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var info in fields)
            {
                var instance = new T();
                if (info.GetValue(instance) is T locatedValue)
                {
                    yield return locatedValue;
                }
            }
        }

        public static T GetByDisplayName<T>(string displayName) where T : Enumeration, new()
        {
            var value = GetAll<T>().SingleOrDefault(x => x.DisplayName == displayName);

            if (value == null)
            {
                throw new Exception($"Invalid value for {typeof(T).Name} : '{displayName}'");
            }

            return value;
        }
        public static T GetById<T>(short id) where T : Enumeration, new() => GetAll<T>().SingleOrDefault(x => x.Id == id);
        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
        public override int GetHashCode() => 2108858624 + Id.GetHashCode();
        public override string ToString() => DisplayName;

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;
            if (otherValue == null)
            {
                return false;
            }
            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);
            return typeMatches && valueMatches;
        }
    }
}
