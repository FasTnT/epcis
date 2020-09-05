using FasTnT.Model.Enums;
using System.Collections.Generic;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class SimpleParameterFilter<T>
    {
        public EpcisField Field { get; set; }
        public IEnumerable<T> Values { get; set; }
    }
}
