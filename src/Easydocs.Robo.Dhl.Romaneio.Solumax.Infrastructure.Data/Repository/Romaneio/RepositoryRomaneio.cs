using Microsoft.Extensions.Configuration;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Data.Repository.Base;
using System.Threading.Tasks;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Interfaces.IRepository.Invoices;
using System.Collections.Generic;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Settings;
using System;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Data.Repository.Invoices
{

    public class RepositoryRomaneio : RepositoryBase<Domain.Entities.romaneio>, IFindInvoice
    {
        private readonly RoboSolumaxSettings _roboVazFielSettings;        
        public RepositoryRomaneio(IConfiguration configuration
                                , RoboSolumaxSettings roboVazFielSettings) : base(configuration)
        {
            _roboVazFielSettings = roboVazFielSettings;
        }
        public async Task<romaneio> SaveAsync(Domain.Entities.romaneio entity)
        {
            var id = await base.SaveAsync<long, Domain.Entities.romaneio>(entity);
            entity.AssociateId(id);
            return entity;
        }

        public async Task<romaneio> UpdateAsync(Domain.Entities.romaneio entity)
        {
            var id = await  base.UpdateAsync(entity);            
            return entity;
        }

        public async Task<IEnumerable<romaneio>> FindInvoicesPending()
        {
            var result = await base.QueryAsync<romaneio>(_roboVazFielSettings.queryFindInvoicesPending);
            return result;
        }

        public async Task<int> UpdateDateByIdAsync(DateTime date, long id, int Nr_Paginas, string Download)
        {

            //var result = await ExecuteUpdateAsync(_roboVazFielSettings.queryUpdateInvoice,new {id, date, Nr_Paginas, Download});
            var result = await ExecuteUpdateAsync(_roboVazFielSettings.queryUpdateInvoice.Replace("@date", date.ToString("dd/MM/yyyy")), new { id });
            result += await ExecuteUpdateAsync(_roboVazFielSettings.queryUpdateInvoice_2.Replace("@date", date.ToString("dd/MM/yyyy")), new { id });
            return result;
        }
    }
}
