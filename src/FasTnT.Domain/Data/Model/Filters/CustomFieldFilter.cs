using FasTnT.Model;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class CustomFieldFilter
    {
        public CustomField Field { get; set; }
        public object[] Values { get; set; }
        public bool IsInner { get; set; }
    }}
