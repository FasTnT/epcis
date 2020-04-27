using FasTnT.Model.Events;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class ExistCustomFieldFilter
    {
        public CustomField Field { get; set; }
        public bool IsInner { get; set; }
    }
}
