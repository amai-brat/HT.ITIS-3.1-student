using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : 
    CqrsDecorator<CreateUserCommand, Result<CreateUserDto>>, 
    ICommandHandler<CreateUserCommand, CreateUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IEnumerable<IValidator<CreateUserCommand>> validators,
        IPermissionChecker permissionChecker,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork
        ) : base(validators, permissionChecker)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
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

            return new Result<CreateUserDto>(new CreateUserDto(id), true);
        }
        catch (Exception ex)
        {
            return new Result<CreateUserDto>(default, false, ex.Message);
        }
    }
}