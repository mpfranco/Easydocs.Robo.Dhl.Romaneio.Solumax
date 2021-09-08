using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Interfaces.IRepository.Invoices;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Constants;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.DomainObjects;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Messages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.UpdateRomaneio
{
    public class UpdateRomaneioCommandHandler : IRequestHandler<UpdateRomaneioCommand, ResultCommand>
    {
        private readonly IFindInvoice _repository;
        public UpdateRomaneioCommandHandler(IFindInvoice repository)
        {
            _repository = repository;
        }
        public async Task<ResultCommand> Handle(UpdateRomaneioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //var entity = await _repository.UpdateAsync(Parse(request));                
                var result = await _repository.UpdateDateByIdAsync(request.Dt_Download, request.Id, request.Nr_Paginas, request.Download);
                if (result > 0)
                    return new ResultCommand(request, StatusCode.IsSuccess);
                else
                    return new ResultCommand($"Dados não atualizados para NF : {request.NF}", StatusCode.Invalid);
            }
            catch (Exception err)
            {
                return new ResultCommand(err.Message, StatusCode.Invalid);
            }
        }

        private Domain.Entities.romaneio Parse(UpdateRomaneioCommand command)
        {
            return new Domain.Entities.romaneio(command.Id,
                               command.Dt_Download,
                               command.NF,
                               command.CNPJ,
                               command.Serie,
                               command.Download,
                               command.Emissao,
                               command.Nr_romaneio
                              );
        }
    }
}
