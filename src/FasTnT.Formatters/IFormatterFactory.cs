namespace FasTnT.Formatters
{
    public interface IFormatterFactory
    {
        string[] AllowedContentTypes { get; }
        IRequestFormatter RequestFormatter { get; }
        IQueryFormatter QueryFormatter { get; }
        IResponseFormatter ResponseFormatter { get; }
    }
}
