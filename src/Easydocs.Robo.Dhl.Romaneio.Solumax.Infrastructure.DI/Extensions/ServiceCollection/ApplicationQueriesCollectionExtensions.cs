using Microsoft.Extensions.DependencyInjection;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Queries.Occurrences;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class ApplicationQueriesCollectionExtensions
    {
        public static void AddApplicationQueries(this IServiceCollection services)
        {
            services.AddScoped<IFindInvoiceQuerie, FindInvoiceQuerie>();            
        }
    }
}
