using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Domain
{
    public sealed class DomainException : Exception
    {
        public DomainException(string message)
            : base(message)
        {
        }
    }
}
