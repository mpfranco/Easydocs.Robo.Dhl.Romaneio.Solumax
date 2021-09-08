using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Bootstrap.Extensions.ServiceCollection;
using System;
using System.IO;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.CronJob
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private ServiceProvider ServiceProvider { get; }

        public IServiceScope Scope => ServiceProvider.CreateScope();

        public Startup()
        {
            //Obter a env
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //setup our configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            if (!string.IsNullOrWhiteSpace(envName))
                builder.AddJsonFile($"appsettings.{envName}.json", optional: true);            

            Configuration = builder.Build();

            //setup our DI
            var servicesBuilder = new ServiceCollection()
                .AddLogging(config =>
                {
                    config.AddConfiguration(Configuration.GetSection("Logging"));
                    config.AddConsole();
                    config.AddDebug();
                });

            ConfigureServices(servicesBuilder);

            ServiceProvider = servicesBuilder.BuildServiceProvider();
        }

        public void ConfigureServices(IServiceCollection services)
        {           
            //services.AddScoped<RestClient>();
            services.AddScoped(x => { return Configuration; });
            services.AddRepository(Configuration);
            services.AddDependencyInjection();
            services.AddSettings(Configuration);
            services.AddBus();
            services.AddApplicationQueries();
            services.AddPolicies();

        }

    }
}