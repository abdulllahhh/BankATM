using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Domain.Common
{
    public sealed class BusinessRuleValidationException
        : DomainException
    {
        public BusinessRuleValidationException(
            IBusinessRule rule)
            : base(rule.Message)
        {
        }
    }
}
