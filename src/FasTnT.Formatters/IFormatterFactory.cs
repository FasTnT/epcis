namespace FasTnT.Formatters
{
    public interface IFormatterFactory
    {
        string ContentType { get; }
        IRequestFormatter RequestFormatter { get; }
        IQueryFormatter QueryFormatter { get; }
        IResponseFormatter ResponseFormatter { get; }
    }
}
