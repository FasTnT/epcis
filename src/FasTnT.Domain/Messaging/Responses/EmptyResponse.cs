namespace FasTnT.Commands.Responses
{
    public class EmptyResponse : IEpcisResponse
    {
        public static IEpcisResponse Value = new EmptyResponse();
    }
}
