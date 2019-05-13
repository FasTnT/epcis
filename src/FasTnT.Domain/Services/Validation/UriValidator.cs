using FasTnT.Model.Exceptions;
using System.Text.RegularExpressions;

namespace FasTnT.Domain.Services
{
    public static class UriValidator
    {
        private static readonly RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
        private static readonly Regex UriRegex = new Regex(@"^\w+:(\/?\/?)[^\s]+?$", Options);
        private static readonly Regex HttpRegex = new Regex(@"^(https?):(\/\/)[^\s]+?$", Options);

        public static void Validate(string uri, bool httpOnly = false)
        {
            if (!(httpOnly ? HttpRegex : UriRegex).IsMatch(uri)) throw new EpcisException(ExceptionType.InvalidURIException, $"URI not valid: '{uri}'");
        }
    }
}
