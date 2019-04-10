namespace FasTnT.Model.Exceptions
{
    public class ContentTypeException : EpcisException
    {
        public ContentTypeException(string message) : base(ExceptionType.ImplementationException, message, ExceptionSeverity.Error)
        {
        }
    }
}
