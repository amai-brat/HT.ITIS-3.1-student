using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .EmailAddress().WithMessage("Email is invalid")
            .MustAsync(IsUniqueEmailAsync).WithMessage("Email already exists");
        
        RuleFor(x => x.Name)
            .Length(3, 100).WithMessage("Name must be between 3 and 100 characters");
    }

    private async Task<bool> IsUniqueEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetUsersAsync(cancellationToken);
        return !users.Any(u => u.Email == email);
    }
}