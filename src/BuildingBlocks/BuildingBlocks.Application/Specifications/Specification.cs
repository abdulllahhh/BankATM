using System.Linq.Expressions;

namespace BuildingBlocks.Application.Specifications;

/// <summary>
/// Base class for query specifications. Stores filtering, inclusion, ordering, and paging metadata.
/// </summary>
public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>>? Criteria { get; }

    private readonly List<Expression<Func<T, object>>> _includes = [];
    public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes.AsReadOnly();

    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    public int Skip { get; private set; }
    public int Take { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Adds a related entity to be included in the query result.
    /// </summary>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
        => _includes.Add(includeExpression);

    /// <summary>
    /// Sets ascending order on the query.
    /// </summary>
    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        => OrderBy = orderByExpression;

    /// <summary>
    /// Sets descending order on the query.
    /// </summary>
    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        => OrderByDescending = orderByDescendingExpression;

    /// <summary>
    /// Enables paging with the specified skip and take values.
    /// </summary>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}
