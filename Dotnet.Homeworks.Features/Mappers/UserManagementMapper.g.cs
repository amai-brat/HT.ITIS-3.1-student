using System;
using System.Linq;
using System.Linq.Expressions;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.UserManagement.Mapping;
using Dotnet.Homeworks.Features.UserManagement.Queries.GetAllUsers;

namespace Dotnet.Homeworks.Features.UserManagement.Mapping
{
    public partial class UserManagementMapper : IUserManagementMapper
    {
        public Expression<Func<User, GetUserDto>> ProjectToGetUserDto => p1 => new GetUserDto(p1.Id, p1.Name, p1.Email);
        public GetAllUsersDto MapToGetAllUsersDto(IQueryable<User> x)
        {
            return new GetAllUsersDto(x.Select<User, GetUserDto>(u => new GetUserDto(u.Id, u.Name, u.Email)));
        }
    }
}