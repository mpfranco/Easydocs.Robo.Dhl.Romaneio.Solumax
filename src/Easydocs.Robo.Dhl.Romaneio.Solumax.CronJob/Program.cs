using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Services;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.CronJob
{
    class Program
    {
        protected Program() { }
        public static async Task Main()
        {
            var startUp = new Startup();

            using (var scope = startUp.Scope)
            {
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();

                logger.LogInformation("Easydocs.Robo.Dhl.Romaneio.Solumax.Cronjob iniciado!");

                await startUp.Scope.ServiceProvider.GetService<IFindRomaneioService>().Executar();

                logger.LogInformation("Easydocs.Robo.Dhl.Romaneio.Solumax.Cronjob finalizado!");
            }
        }
    }
}
