﻿using System.Linq.Expressions;
using Dotnet.Homeworks.DataAccess.Helpers;

namespace Dotnet.Homeworks.DataAccess.Specs.Infrastructure;

public sealed class Specification<T> : IQueryableFilter<T> 
    where T : class
{
    public Expression<Func<T, bool>> Expression { get; }

    public Specification(Expression<Func<T, bool>> expression)
    {
        Expression = expression;
        if (expression == null) throw new ArgumentNullException(nameof(expression));
    }

    public static implicit operator Expression<Func<T, bool>>(Specification<T> spec)
        => spec.Expression;

    public static bool operator false(Specification<T> _)
    {
        return false;
    }

    public static bool operator true(Specification<T> _)
    {
        return false;
    }

    public static Specification<T> operator &(Specification<T> spec1, Specification<T> spec2)
        => new(spec1.Expression.And(spec2.Expression));

    public static Specification<T> operator |(Specification<T> spec1, Specification<T> spec2)
        => new(spec1.Expression.Or(spec2.Expression));

    public static Specification<T> operator !(Specification<T> spec)
        => new(spec.Expression.Not());

    public IQueryable<T> Apply(IQueryable<T> query)
        => query.Where(Expression);

    public bool IsSatisfiedBy(T obj) => Expression.Compile()(obj);
}