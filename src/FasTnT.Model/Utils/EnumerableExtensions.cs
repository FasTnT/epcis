using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Model
{
    public static class EnumerableExtensions
    {
        public static T SingleWhenOnly<T>(this IEnumerable<T> collection)
        {
            return (collection.Count() == 1) ? collection.First() : default(T);
        }
    }
}
