using Dotnet.Homeworks.MainProject.Configuration;
using MassTransit;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasstransitRabbitMq(this IServiceCollection services,
        RabbitMqConfig rabbitConfiguration)
    {
        services.AddMassTransit(busConfigurator =>
        {
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