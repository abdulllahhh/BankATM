using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Domain.Common
{
    public interface IBusinessRule
    {
        bool IsBroken();

        string Message { get; }
    }
}
