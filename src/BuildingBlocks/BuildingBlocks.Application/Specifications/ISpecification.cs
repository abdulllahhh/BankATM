using System.Linq.Expressions;

namespace BuildingBlocks.Application.Specifications;

/// <summary>
/// Defines a query specification with filtering, inclusion, ordering, and paging.
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// The filtering criteria for the query.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Related entities to include in the query result.
    /// </summary>
    IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// The ascending order expression.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// The descending order expression.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// The number of items to skip.
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// The number of items to take.
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Whether paging is enabled.
    /// </summary>
    bool IsPagingEnabled { get; }
}
