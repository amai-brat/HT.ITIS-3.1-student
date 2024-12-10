using Dotnet.Homeworks.Features.Helpers;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;
using Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;
using FluentValidation;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        
        services.AddHttpContextAccessor();
        services.AddPermissionChecks(AssemblyReference.Assembly);
        
        services.AddMediator(AssemblyReference.Assembly);
        services.AddPipelineBehaviorsForFeaturesRequests(
            AssemblyReference.Assembly, 
            Dotnet.Homeworks.Infrastructure.Helpers.AssemblyReference.Assembly);

        return services;
    }
}