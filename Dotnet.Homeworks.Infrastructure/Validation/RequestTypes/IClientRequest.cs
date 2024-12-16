namespace Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;

public interface IClientRequest : ISecuredRequest
{
    public Guid Guid { get; }
}