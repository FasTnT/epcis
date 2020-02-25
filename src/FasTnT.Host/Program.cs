using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace FasTnT.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

            BuildWebHost(args, configuration).Run();
        }
        public static IWebHost BuildWebHost(string[] args, IConfiguration configuration) => 
            WebHost.CreateDefaultBuilder(args)
            .UseShutdownTimeout(TimeSpan.FromSeconds(10))
            .UseKestrel(c => c.AddServerHeader = false)
            .UseConfiguration(configuration)
            .UseStartup<Startup>()
            .Build();
    }
}
