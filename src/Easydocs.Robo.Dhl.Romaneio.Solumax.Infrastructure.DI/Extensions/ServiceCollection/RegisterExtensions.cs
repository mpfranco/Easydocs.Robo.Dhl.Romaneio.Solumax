using Microsoft.Extensions.DependencyInjection;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Services;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Services;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Services;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class RegisterExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IFindRomaneioService, FindRomaneioService>();
            services.AddScoped<ILoggerRomaneio, LoggerRomaneio>();  
            return services;
        }
    }
}