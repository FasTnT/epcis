using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace FasTnT.Handlers.DatabaseSetup
{
    public static class DatabaseUtils
    {
        public static async Task<string> UnzipCommand(string zippedCommand)
        {
            using (var reader = new StreamReader(new GZipStream(new MemoryStream(Convert.FromBase64String(zippedCommand)), CompressionMode.Decompress)))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
