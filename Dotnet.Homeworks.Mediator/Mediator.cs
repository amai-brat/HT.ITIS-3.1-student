using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Mediator;

public class Mediator : IMediator
{
    private class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        // ReSharper disable once MemberCanBePrivate.Local
        public IServiceProvider ServiceProvider { private get; set; } = null!;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var handler = ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            return await ExecutePipelineBehaviors(
                request,
                () => handler.Handle(request, cancellationToken),
                cancellationToken);
        }
        
        private async Task<TResponse> ExecutePipelineBehaviors(
            TRequest request, 
            RequestHandlerDelegate<TResponse> handler, 
            CancellationToken cancellationToken = default)
        {
            var behaviors = ServiceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .ToList();

            if (behaviors.Count == 0)
            {
                return await handler.Invoke();
            }
        
            var res = await Pipe(request, behaviors, 0, handler, cancellationToken);
            return res;
        }

        private static async Task<TResponse> Pipe(
            TRequest request,
            List<IPipelineBehavior<TRequest, TResponse>> behaviors,
            int index, 
            RequestHandlerDelegate<TResponse> lastHandler,
            CancellationToken cancellationToken = default)
        {
            if (behaviors.Count - 1 == index)
            {
                return await behaviors[index].Handle(request, lastHandler, cancellationToken); 
            }
        
            return await behaviors[index].Handle(
                request, 
                () => Pipe(request, behaviors, index + 1, lastHandler, cancellationToken), 
                cancellationToken);
        }
    }

    private record HandlerMeta(object Handler, MethodInfo HandleMethod, PropertyInfo ServiceProviderProperty);
    
    private static readonly ConcurrentDictionary<Type, HandlerMeta> Handlers = new();
    private readonly IServiceProvider _serviceProvider;
    
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var reqHandlerType = typeof(RequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var (reqHandler, method, prop) = Handlers.GetOrAdd(reqHandlerType, type =>
        {
            var reqHandler = Activator.CreateInstance(type)!;
            var method = type.GetMethod("Handle", BindingFlags.Public | BindingFlags.Instance)!;
            var prop = reqHandlerType.GetProperty("ServiceProvider")!;
            
            return new HandlerMeta(reqHandler, method, prop);
        });
        
        prop.SetValue(reqHandler, _serviceProvider);
        
        var task = (Task<TResponse>)method.Invoke(reqHandler, new object[] { request, cancellationToken })!;
        return await task;
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        var handler = _serviceProvider.GetService<IRequestHandler<TRequest>>()!;
        await handler.Handle(request, cancellationToken);
    }

    public async Task<dynamic?> Send(object request, CancellationToken cancellationToken = default)
    {
        var reqType = request.GetType();
        var reqGenericIface = reqType
            .GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
        var reqIface = reqType
            .GetInterfaces()
            .FirstOrDefault(x => x == typeof(IRequest));
        
        if (reqGenericIface is null && reqIface is null)
        {
            return Task.FromResult<dynamic?>(null);
        }
        
        var sendMethod = typeof(Mediator).GetMethod("Send", BindingFlags.Public | BindingFlags.Instance);
        
        if (reqGenericIface is not null)
        {
            var genericSendMethodWithResponse = sendMethod!.MakeGenericMethod(reqGenericIface.GenericTypeArguments);
            return genericSendMethodWithResponse.Invoke(this, new[] { request, cancellationToken })!;
        }

        var genericSendMethod = sendMethod!.MakeGenericMethod(reqIface!.GenericTypeArguments);
        await (Task)genericSendMethod.Invoke(this, new[] { request, cancellationToken })!;
        return null;
    }
}