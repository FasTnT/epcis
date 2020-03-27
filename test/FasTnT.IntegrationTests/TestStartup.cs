using FasTnT.Host;
using Microsoft.AspNetCore.Hosting;

namespace FasTnT.IntegrationTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IWebHostEnvironment env) : base(env)
        {
        }
    }
}
