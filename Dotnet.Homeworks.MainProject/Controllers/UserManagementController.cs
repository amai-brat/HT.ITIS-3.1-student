using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.UserManagement.Commands.DeleteUserByAdmin;
using Dotnet.Homeworks.Features.UserManagement.Queries.GetAllUsers;
using Dotnet.Homeworks.Features.Users.Commands.CreateUser;
using Dotnet.Homeworks.Features.Users.Commands.DeleteUser;
using Dotnet.Homeworks.Features.Users.Commands.UpdateUser;
using Dotnet.Homeworks.Features.Users.Queries.GetUser;
using Dotnet.Homeworks.Infrastructure.Dto;
using Dotnet.Homeworks.Infrastructure.Services;
using Dotnet.Homeworks.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Homeworks.MainProject.Controllers;

[ApiController]
public class UserManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserManagementController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateUser(RegisterUserDto userDto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateUserCommand(userDto.Name, userDto.Email), cancellationToken);
        return result.IsSuccess 
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("profile/{guid}")]
    public async Task<IActionResult> GetProfile(Guid guid, CancellationToken cancellationToken) 
    {
        var result = await _mediator.Send(new GetUserQuery(guid), cancellationToken);
        return result.IsSuccess 
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpDelete("profile/{guid:guid}")]
    public async Task<IActionResult> DeleteProfile(Guid guid, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteUserCommand(guid), cancellationToken);
        return result.IsSuccess 
            ? NoContent()
            : BadRequest(result.Error);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(User user, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateUserCommand(user), cancellationToken);
        return result.IsSuccess 
            ? NoContent()
            : BadRequest(result.Error);
    }

    [HttpDelete("user/{guid:guid}")]
    public async Task<IActionResult> DeleteUser(Guid guid, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteUserByAdminCommand(guid), cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error);
    }
}