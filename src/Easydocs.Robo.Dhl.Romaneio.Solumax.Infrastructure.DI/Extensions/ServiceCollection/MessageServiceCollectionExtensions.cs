using GreenPipes;
using MassTransit;
using MassTransit.AspNetCoreIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class MessageServiceCollectionExtensions
    {
        public static void AddMessages(this IServiceCollection services, IConfiguration configuration)
        {
            #region Queue
            var queueIUpdateWhiteListConsumer = configuration.GetSection("QueueMessageBus:Consumer:UpdateWhiteListConsumer").Get<string>();
            #endregion

            //services.AddScoped<UpdateWhiteListConsumer>();

            //services.AddScoped<IMessageBus, MassTransitMessageBus>();

            //services.AddMassTransit(provider =>
            //    MessageBusBuilder.CreateUsingConfiguration(
            //        configuration,
            //        services.BuildServiceProvider(),
            //        x => x.Exponential(5, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(2), TimeSpan.FromSeconds(10)))
            //    .UseQueueForSending<IUpdateWhiteListConsumerCommand>(queueIUpdateWhiteListConsumer)
            //    .SetPrefetchCount(1)
            //    .RegisterCommandConsumer<UpdateWhiteListConsumer>(
            //        queueIUpdateWhiteListConsumer,
            //        provider,
            //        y => y.UseConcurrentMessageLimit(1))
            //    .Build(),
            //    x => x.AddConsumer<UpdateWhiteListConsumer>(y =>
            //    {
            //        y.UseConcurrentMessageLimit(1);
            //        y.UseConcurrencyLimit(1);
            //    }));
        }
    }
}