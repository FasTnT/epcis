using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            var host = BuildWebHost(args, configuration);
            host.Run();
        }
        public static IHost BuildWebHost(string[] args, IConfiguration configuration) =>
            new HostBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseShutdownTimeout(TimeSpan.FromSeconds(10))
                          .UseIISIntegration()
                          .ConfigureLogging((config, builder) =>
                          {
                              builder.AddConsole()
                                     .SetMinimumLevel(LogLevel.Debug);
                          })
                          .UseConfiguration(configuration)
                          .UseStartup<Startup>();
            })
            .Build();
    }
}