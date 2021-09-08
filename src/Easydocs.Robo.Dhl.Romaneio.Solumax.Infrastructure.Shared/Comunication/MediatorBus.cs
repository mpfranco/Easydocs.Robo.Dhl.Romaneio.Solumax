using MediatR;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.DomainObjects;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Messages;
using System.Threading.Tasks;


namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Comunication
{
    public class MediatorBus : IMediatorBus
    {
        private readonly IMediator _mediator;
        public MediatorBus(IMediator mediator)
        {
            _mediator = mediator;
        }
        public Task PublishEventAsync<T>(T @event) where T : Event => _mediator.Publish(@event);
        public Task<ResultCommand> SendCommadAsync<T>(T command) where T : Command => _mediator.Send(command);
        public Task<ResultCommand<T2>> SendCommadAsync<T, T2>(T command) where T : Command<T2> => _mediator.Send(command);
    }
}
