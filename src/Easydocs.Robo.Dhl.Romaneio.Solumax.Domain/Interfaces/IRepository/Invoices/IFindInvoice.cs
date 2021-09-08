using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Interfaces.IRepository.Base;
using System.Threading.Tasks;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities;
using System.Collections.Generic;
using System;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Interfaces.IRepository.Invoices
{
    public interface IFindInvoice : IRepositoryBase<Entities.romaneio>
    {
        Task<Entities.romaneio> SaveAsync(Entities.romaneio entity);
        Task<Entities.romaneio> UpdateAsync(Entities.romaneio entity);
        Task<int> UpdateDateByIdAsync(DateTime date, long id, int nr_Paginas, string Download);
        Task<IEnumerable<Entities.romaneio>> FindInvoicesPending();
    }
}

