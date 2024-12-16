using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Users.Commands.CreateUser;
using Dotnet.Homeworks.Infrastructure.Dto;
using Mapster;

namespace Dotnet.Homeworks.Features.Users.Mapping;

[Mapper]
public interface IUserMapper
{
    User MapToUser(CreateUserCommand command);
    RegisterUserDto MapToRegisterUserDto(CreateUserCommand command);
}