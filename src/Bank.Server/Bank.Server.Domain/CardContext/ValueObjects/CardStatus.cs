using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.CardContext.ValueObjects
{
    public enum CardStatus
    {
        Lost,
        Stolen,
        Confiscated,
        Active
    }
}
