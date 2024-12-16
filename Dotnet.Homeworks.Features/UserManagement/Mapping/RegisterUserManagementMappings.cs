using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.UserManagement.Queries.GetAllUsers;
using Mapster;
using Mapster.Models;

namespace Dotnet.Homeworks.Features.UserManagement.Mapping;

public class RegisterUserManagementMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, GetUserDto>()
            .Map(x => x.Guid, x => x.Id)
            .RequireDestinationMemberSource(true);

        // запрос в бд только с нужными полями, но если у Dto много полей, это плохой маппинг
        config.NewConfig<IQueryable<User>, GetAllUsersDto>()
            .MapWith(x => new GetAllUsersDto(x
                .Select(u => new GetUserDto(u.Id, u.Name, u.Email))));
        
        // запрос в бд со всеми полями
        // config.NewConfig<IQueryable<User>, GetAllUsersDto>()
        //     .MapWith(x => new GetAllUsersDto(x
        //         .Select(u => u.Adapt<GetUserDto>())));
        
        // запрос в бд со всеми полями
        // config.NewConfig<IQueryable<User>, GetAllUsersDto>()
        //     .Map(x => x.Users, x => x);
    }
}