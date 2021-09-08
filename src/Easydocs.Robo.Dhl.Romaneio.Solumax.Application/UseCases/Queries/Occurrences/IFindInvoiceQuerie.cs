using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Queries.Occurrences
{
    public interface IFindInvoiceQuerie
    {
        Task<IEnumerable<Domain.Entities.romaneio>> FindInvoicesPending();
    }
}
