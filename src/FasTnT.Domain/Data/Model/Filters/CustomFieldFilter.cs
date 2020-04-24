using FasTnT.Model.Events;
using System.Collections.Generic;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class CustomFieldFilter
    {
        public CustomField Field { get; set; }
        public IList<object> Values { get; set; }
        public bool IsInner { get; set; }
    }}
