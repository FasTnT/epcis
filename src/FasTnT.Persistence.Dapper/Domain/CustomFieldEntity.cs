using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class CustomFieldEntity : CustomField
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int EventId { get; set; }
    }
}
