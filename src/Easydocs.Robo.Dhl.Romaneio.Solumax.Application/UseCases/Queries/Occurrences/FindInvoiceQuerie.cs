
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Interfaces.IRepository.Invoices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Queries.Occurrences
{
    public class FindInvoiceQuerie : IFindInvoiceQuerie
    {
        private readonly IFindInvoice _repository;
        public FindInvoiceQuerie(IFindInvoice repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Domain.Entities.romaneio>> FindInvoicesPending()
        {
            try
            {
                return await _repository.FindInvoicesPending();
            }
            catch(Exception err)
            {
                return null;
            }
            

        }
    }
}
