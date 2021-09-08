using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Super.IS2B.Occurrency.Infrastructure.Bootstrap;

namespace Super.IS2B.Occurrency.Consumer
{
    public class Startup
    {
        private readonly ApplicationStartup _startup;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _startup = new ApplicationStartup(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _startup.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app) => _startup.Configure(app);
    }
}