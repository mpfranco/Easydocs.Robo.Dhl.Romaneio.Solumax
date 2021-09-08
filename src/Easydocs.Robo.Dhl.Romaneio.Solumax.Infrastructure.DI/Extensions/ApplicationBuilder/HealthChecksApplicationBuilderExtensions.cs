using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Bootstrap.Extensions.ApplicationBuilder
{
    public static class HealthChecksApplicationBuilderExtensions
    {
        public static void UseCustomHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            //Será utilizado para fazer o readnessProbe/liveness do k8s
            app.UseHealthChecks("/liveness", new HealthCheckOptions()
            {
                Predicate = _ => true,
            });
        }
    }
}
