using Dotnet.Homeworks.Storage.API.Configuration;
using Microsoft.Extensions.Options;
using Minio;

namespace Dotnet.Homeworks.Storage.API.ServicesExtensions;

public static class AddMinioExtensions
{
    public static IServiceCollection AddMinioClient(this IServiceCollection services,
        MinioConfig? conf = default)
    {
        if (conf == null)
        {
            services.AddSingleton<IMinioClient, MinioClient>(sp =>
            {
                conf = sp.GetService<IOptionsMonitor<MinioConfig>>()!.CurrentValue;
                return GetMinioClient(conf);
            });
        }
        else
        {
            services.AddSingleton<IMinioClient, MinioClient>(_ => GetMinioClient(conf));
        }

        return services;
    }

    private static MinioClient GetMinioClient(MinioConfig conf)
    {
        return new MinioClient()
            .WithCredentials(conf.Username, conf.Password)
            .WithEndpoint(conf.Endpoint, conf.Port)
            .WithSSL(conf.WithSsl)
            .Build();
    }
}