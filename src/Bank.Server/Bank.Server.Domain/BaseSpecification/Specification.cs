using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.BaseSpecification
{
    public abstract class Specification<T>
    {
        public abstract bool IsSatisfiedBy(T entity);
    }
}
