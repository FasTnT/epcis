namespace FasTnT.Model.Responses
{
    public class ExceptionResponse : IEpcisResponse
    {
        public string Exception { get; set; }
        public string Reason { get; set; }
    }
}
