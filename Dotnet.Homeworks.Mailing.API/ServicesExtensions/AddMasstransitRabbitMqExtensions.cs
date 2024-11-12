using Dotnet.Homeworks.Mailing.API.Configuration;
using Dotnet.Homeworks.Mailing.API.Consumers;
using MassTransit;

namespace Dotnet.Homeworks.Mailing.API.ServicesExtensions;

public static class AddMasstransitRabbitMqExtensions
{
    public static IServiceCollection AddMasstransitRabbitMq(this IServiceCollection services,
        RabbitMqConfig rabbitConfiguration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<EmailConsumer>();
            
            busConfigurator.UsingRabbitMq((ctx, configurator) =>
            {
                configurator.Host(rabbitConfiguration.Hostname, "/",conf =>
                {
                    conf.Username(rabbitConfiguration.Username);
                    conf.Password(rabbitConfiguration.Password);
                });
                
                configurator.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}