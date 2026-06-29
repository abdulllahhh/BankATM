using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BuildingBlocks.Domain.Specifications
{
    public abstract class Specification<T> : ISpecification<T>
    {
        public abstract Expression<Func<T, bool>> Criteria { get; }
    }
}
