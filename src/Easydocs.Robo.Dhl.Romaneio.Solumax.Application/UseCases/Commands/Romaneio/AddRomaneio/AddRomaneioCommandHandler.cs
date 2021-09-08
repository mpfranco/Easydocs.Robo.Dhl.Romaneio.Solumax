using MediatR;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Constants;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.DomainObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Interfaces.IRepository.Invoices;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.AddRomaneio
{
    public class AddRomaneioCommandHandler : IRequestHandler<AddRomaneioCommand, ResultCommand>
    {
        private readonly IFindInvoice _repository;
        public AddRomaneioCommandHandler(IFindInvoice repository)
        {
            _repository = repository;
        }
        public async Task<ResultCommand> Handle(AddRomaneioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.SaveAsync(Parse(request));
                request.AssociateId(entity.Id);
                return new ResultCommand(request, StatusCode.IsSuccess);
            }
            catch (Exception err)
            {
                return new ResultCommand(err.Message, StatusCode.Invalid);
            }


        }


        private Domain.Entities.romaneio Parse(AddRomaneioCommand command)
        {
            return new Domain.Entities.romaneio(command.Dt_Download,
                                 command.NF,
                                 command.CNPJ,
                                 command.Serie,
                                 command.Nr_romaneio
                                );
        }
    }
}
