using FasTnT.Host;
using Microsoft.Extensions.Configuration;

namespace FasTnT.IntegrationTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration config) : base(config)
        {
        }
    }
}
