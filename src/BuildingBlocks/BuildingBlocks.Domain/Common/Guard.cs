using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Domain.Common
{
    public static class Guard
    {
        public static void CheckRule(
            IBusinessRule rule)
        {
            if (rule.IsBroken())
                throw new BusinessRuleValidationException(rule);
        }
    }
}
