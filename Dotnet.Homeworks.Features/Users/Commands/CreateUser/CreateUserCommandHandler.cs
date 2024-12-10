using System.Security.Claims;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.Dto;
using Dotnet.Homeworks.Infrastructure.Services;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.Enums;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : 
    CqrsDecorator<CreateUserCommand, Result<CreateUserDto>>, 
    ICommandHandler<CreateUserCommand, CreateUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRegistrationService _registrationService;
    private readonly HttpContext _httpContext;

    public CreateUserCommandHandler(
        IEnumerable<IValidator<CreateUserCommand>> validators,
        IPermissionChecker permissionChecker,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IRegistrationService registrationService,
        IHttpContextAccessor httpContextAccessor
        ) : base(validators, permissionChecker)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _registrationService = registrationService;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public override async Task<Result<CreateUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var res = await base.Handle(request, cancellationToken);
        if (res.IsFailure)
        {
            return res;
        }

        try
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email
            };

            var id = await _userRepository.InsertUserAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, id.ToString()),
                new(ClaimTypes.Role, Roles.Admin.ToString()),
                new(ClaimTypes.Role, Roles.User.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _httpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
            
            await _registrationService.RegisterAsync(new RegisterUserDto(request.Name, request.Email));

            return new Result<CreateUserDto>(new CreateUserDto(id), true);
        }
        catch (Exception ex)
        {
            return new Result<CreateUserDto>(default, false, ex.Message);
        }
    }
}