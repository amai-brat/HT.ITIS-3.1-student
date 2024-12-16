using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Users.Commands.CreateUser;
using Dotnet.Homeworks.Features.Users.Mapping;
using Dotnet.Homeworks.Infrastructure.Dto;

namespace Dotnet.Homeworks.Features.Users.Mapping
{
    public partial class UserMapper : IUserMapper
    {
        public User MapToUser(CreateUserCommand p1)
        {
            return p1 == null ? null : new User()
            {
                Email = p1.Email,
                Name = p1.Name
            };
        }
        public RegisterUserDto MapToRegisterUserDto(CreateUserCommand p2)
        {
            return p2 == null ? null : new RegisterUserDto(p2.Name, p2.Email);
        }
    }
}