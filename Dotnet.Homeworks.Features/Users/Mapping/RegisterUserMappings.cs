using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Users.Commands.CreateUser;
using Dotnet.Homeworks.Infrastructure.Dto;
using Mapster;

namespace Dotnet.Homeworks.Features.Users.Mapping;

public class RegisterUserMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateUserCommand, User>();
        config.NewConfig<CreateUserCommand, RegisterUserDto>();
    }
}