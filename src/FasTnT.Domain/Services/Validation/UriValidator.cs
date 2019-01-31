using System;
using System.Text.RegularExpressions;

namespace FasTnT.Domain.Services
{
    public static class UriValidator
    {
        private static readonly Regex UriRegex = new Regex(@"^\w+:(\/?\/?)[^\s]+?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex HttpRegex = new Regex(@"^(https?):(\/\/)[^\s]+?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static void Validate(string uri, bool httpOnly = false)
        {
            if (!(httpOnly ? HttpRegex : UriRegex).IsMatch(uri)) throw new Exception($"URI not valid: '{uri}'");
        }
    }
}
