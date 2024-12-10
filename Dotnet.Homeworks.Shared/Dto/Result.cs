using System.Collections.Concurrent;
using System.Reflection;
using BindingFlags = System.Reflection.BindingFlags;

namespace Dotnet.Homeworks.Shared.Dto;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    public Result(bool isSuccessful, string? error = default)
    {
        IsSuccess = isSuccessful;
        if (error is not null) 
            Error = error;
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? val, bool isSuccessful, string? error = default)
        : base(isSuccessful, error)
    {
        _value = val;
    }

    public TValue? Value => IsSuccess
        ? _value
        : throw new Exception(Error);
}

public static class ResultFactory
{
    private static readonly ConcurrentDictionary<Type, ConstructorInfo> CtorCache = new();
    
    public static object Create(
        bool isSuccessfull, 
        Type type, 
        object? value = default, 
        string? error = default)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var ctor = CtorCache.GetOrAdd(type, t =>
            {
                var genericType = t.GetGenericArguments()[0];
                return t.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                    new[] { genericType, typeof(bool), typeof(string) })!;
            });
            
            var result = ctor.Invoke(new []{value, isSuccessfull, error});
            
            return result;
        }

        return type.IsAssignableTo(typeof(Result))
            ? new Result(isSuccessfull, error)
            : throw new InvalidOperationException($"{type.FullName} is not a Result type");
    }
}