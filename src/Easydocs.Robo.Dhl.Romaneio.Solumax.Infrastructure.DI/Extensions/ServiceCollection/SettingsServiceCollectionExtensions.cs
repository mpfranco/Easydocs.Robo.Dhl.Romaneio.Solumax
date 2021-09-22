using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Settings;


namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class SettingsServiceCollectionExtensions
    {
        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            //Settings
            services.Configure<RoboSolumaxSettings>(configuration.GetSection("Robo.Solumax"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<RoboSolumaxSettings>>().Value);
            
        }
    }
}
