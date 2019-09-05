using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class EpcisEventEntity : EpcisEvent
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
    }
}
