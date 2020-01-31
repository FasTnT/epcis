using FasTnT.Model;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class ExistCustomFieldFilter
    {
        public CustomField Field { get; set; }
        public bool IsInner { get; set; }
    }
}
