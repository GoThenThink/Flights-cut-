using RandomStartup.Extensions.AspNetCore.Consul;
using RandomStartup.Extensions.AspNetCore.Vault;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Flights
{
    /// <summary/>
    internal sealed class Program
    {
        /// <summary/>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary/>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var result = Host.CreateDefaultBuilder(args);

            if (environment == Environments.Development)
            {
                return result.ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            }

            return result
                .ConfigureVault()
                .ConfigureConsul(config => { config.ConsulConfigSectionName = "flights/ConsulConfig"; })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}
