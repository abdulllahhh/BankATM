using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Domain.Common
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            return GetEqualityComponents()
                .SequenceEqual(((ValueObject)obj).GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(
                    0,
                    (current, obj) =>
                        HashCode.Combine(current, obj));
        }
    }
}
