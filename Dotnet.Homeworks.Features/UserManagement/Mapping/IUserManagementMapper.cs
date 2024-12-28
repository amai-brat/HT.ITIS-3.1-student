using System.Linq.Expressions;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.UserManagement.Queries.GetAllUsers;
using Mapster;

namespace Dotnet.Homeworks.Features.UserManagement.Mapping;

[Mapper]
public interface IUserManagementMapper
{
    Expression<Func<User, GetUserDto>> ProjectToGetUserDto { get; }
    
    // есть тест на наличие метода у маппера с параметром IQueryable,
    // репозитории в других фичах возвращают IEnumerable, 
    // так что тут самое подходящее место.
    // но я не смог сделать так, чтобы в бд отправлялся запрос только с нужными полями,
    // поэтому этот метод не используется
    GetAllUsersDto MapToGetAllUsersDto(IQueryable<User> users);
}