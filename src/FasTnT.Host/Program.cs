using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace FasTnT.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            host.Run();
        }

        public static IHost BuildWebHost(string[] args) =>
            new HostBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseShutdownTimeout(TimeSpan.FromSeconds(10))
                          .UseIISIntegration()
                          .ConfigureKestrel(opt => opt.AddServerHeader = false)
                          .ConfigureLogging((config, builder) =>
                          {
                              builder.AddConsole()
                                     .SetMinimumLevel(LogLevel.Debug);
                          })
                          .ConfigureAppConfiguration((hostingContext, config) =>
                          {
                              config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                              config.AddEnvironmentVariables();
                              config.AddCommandLine(args);

                              if (hostingContext.HostingEnvironment.IsDevelopment())
                              {
                                  config.AddUserSecrets<Startup>();
                              }
                          })
                          .UseStartup<Startup>();
            })
            .Build();
    }
}