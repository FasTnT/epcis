namespace FasTnT.Commands.Responses
{
    public class EmptyResponse : IEpcisResponse
    {
        public static IEpcisResponse Default = new EmptyResponse();
    }
}
