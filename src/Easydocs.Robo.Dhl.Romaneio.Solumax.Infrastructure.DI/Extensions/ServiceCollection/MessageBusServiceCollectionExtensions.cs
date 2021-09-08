using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Comunication;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.DomainObjects;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.AddRomaneio;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.UpdateRomaneio;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class MessageBusServiceCollectionExtensions
    {
        public static void AddBus(this IServiceCollection services)
        {
            //  Bus (Mediator)
            services.AddMediatR(typeof(ApplicationStartup));
            services.AddScoped<IMediatorBus, MediatorBus>();
            services.AddScoped<IRequestHandler<AddRomaneioCommand, ResultCommand>, AddRomaneioCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateRomaneioCommand, ResultCommand>, UpdateRomaneioCommandHandler>();

        }
    }
}
