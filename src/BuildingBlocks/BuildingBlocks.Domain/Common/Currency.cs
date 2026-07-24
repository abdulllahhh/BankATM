using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Domain.Common
{
    public sealed class Currency : ValueObject
    {
        public string Code { get; }

        private Currency(string code)
        {
            Code = code;
        }

        public static readonly Currency EGP = new("EGP");
        public static readonly Currency USD = new("USD");

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Code;
        }
    }
}
