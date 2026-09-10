using Neo.Domain.Features.PubSub;
using Hyper.Infrastructure.Features.Sms.MassTransit;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Hyper.Infrastructure.Features.PubSub.MassTransit;

public static class DependencyInjection
{
    public static IServiceCollection AddMassTransitServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INeoPublisher, MassTransitNeoPublisher>();
        //TODO Quartz? 
        services.AddQuartz();
        services.AddQuartzHostedService();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            //x.AddPublishMessageScheduler();
            //x.AddQuartzConsumers();

            x.AddConsumer<OtpSmsSentConsumer>();
            //x.AddConsumer<FcmRegisterConsumer>();
            //x.AddConsumer<FcmPushConsumer>();
            var env = configuration["App:Environment"];
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host($"rabbitmq://{configuration["Rabbit:Url"]}", h =>
                {
                    h.Username(configuration["Rabbit:Username"]!);
                    h.Password(configuration["Rabbit:Password"]!);
                });
                cfg.ReceiveEndpoint("otp-sms-sent", e =>
                {
                    e.ConfigureConsumer<OtpSmsSentConsumer>(context);
                    e.DiscardSkippedMessages();
                });
                //cfg.UsePublishMessageScheduler();
                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
