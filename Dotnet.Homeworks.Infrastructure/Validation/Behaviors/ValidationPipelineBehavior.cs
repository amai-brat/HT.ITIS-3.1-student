using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        
        var validationResultTasks = _validators
            .Select(x => x.ValidateAsync(request, cancellationToken));
        var validationResult = await Task.WhenAll(validationResultTasks);
        
        if (!validationResult.Any(x => x.IsValid))
        {
            var failures = validationResult.SelectMany(x => x.Errors);
            return (TResponse)ResultFactory.Create(
                false, 
                typeof(TResponse),
                null,
                string.Join(";", failures.Select(x => x.ErrorMessage)));
        }
        
        return await next();
    }
}