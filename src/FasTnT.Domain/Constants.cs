using System.Reflection;

namespace FasTnT.Domain
{
    public static class Constants
    {
        public const string VendorVersion = "2.1.3";
        public const string StandardVersion = "1.2";

        public static readonly Assembly Assembly = typeof(Constants).Assembly;
    }
}
