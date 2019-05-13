namespace FasTnT.Model
{
    public abstract class Request : IEpcisPayload
    {
        public EpcisRequestHeader Header { get; set; }
    }
}
