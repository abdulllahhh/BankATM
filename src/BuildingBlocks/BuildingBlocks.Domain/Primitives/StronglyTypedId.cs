using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Domain.Primitives
{
    public abstract record StronglyTypedId(Guid Value);
}
