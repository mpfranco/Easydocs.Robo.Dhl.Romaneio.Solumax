using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Super.Package.Configuration.ConfigServer;
using Super.Package.Configuration.ConfigServer.Abstractions;

namespace Super.IS2B.Occurrency.Consumer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var metrics = AppMetrics.CreateDefaultBuilder()
              .OutputMetrics.AsPrometheusPlainText()
              .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseMetrics(
                            options =>
                            {
                                options.EndpointOptions = endpointsOptions =>
                                {
                                    endpointsOptions.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters.GetType<MetricsPrometheusTextOutputFormatter>();
                                };
                            })
                            .ConfigureAppConfiguration((context, builder) =>
                            {
                                if (context.HostingEnvironment.EnvironmentName == "Local")
                                {
                                    return;
                                }
                                builder.AddConfigServer(context.HostingEnvironment.EnvironmentName);
                            })
                .UseStartup<Startup>();
        }
    }
}