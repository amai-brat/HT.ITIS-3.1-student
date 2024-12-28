using Dotnet.Homeworks.MainProject.Configuration;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.OpenTelemetry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services,
        OpenTelemetryConfig openTelemetryConfiguration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(rb =>
            {
                rb.AddService(
                    serviceName: DiagnosticConfig.ServiceName,
                    autoGenerateServiceInstanceId: true);
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(ef =>
                    {
                        ef.EnrichWithIDbCommand = (activity, command) =>
                        {
                            activity.SetTag("db.command_text", command.CommandText);
                        };
                    });
                
                tracing.AddOtlpExporter(conf =>
                {
                    conf.Endpoint = new Uri(openTelemetryConfiguration.OtlpExporterEndpoint);
                });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
                metrics.AddMeter(DiagnosticConfig.Meter.Name);
                
                metrics.AddConsoleExporter();
            });
        
        return services;
    }
}