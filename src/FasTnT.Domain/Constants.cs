using System.Reflection;

namespace FasTnT.Domain
{
    public static class Constants
    {
        public static string VendorVersion => Assembly.GetName().Version.ToString(3);
        public static string StandardVersion => "1.2";

        public static readonly Assembly Assembly = typeof(Constants).Assembly;
    }
}
