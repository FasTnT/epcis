using FasTnT.Model.Exceptions;
using System.Text.RegularExpressions;

namespace FasTnT.Domain
{
    public static class UriValidator
    {
        private static readonly Regex HttpRegex = new Regex(@"^(https?):(\/\/)[^\s]+?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static void Validate(string uri)
        {
            if (!HttpRegex.IsMatch(uri)) throw new EpcisException(ExceptionType.InvalidURIException, $"URI not valid: '{uri}'");
        }
    }
}
