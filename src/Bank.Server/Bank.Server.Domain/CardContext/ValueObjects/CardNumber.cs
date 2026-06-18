using Bank.Server.Domain.AccountContext.ValueObjects;
using Bank.Server.Domain.BaseValueObject;
using BuildingBlocks.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.CardContext.ValueObjects
{
    public sealed class CardNumber(string Value) : ValueObject
    {
        public string Value { get; }
        public static CardNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Invalid account number");

            return new CardNumber(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
