using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.AccountContext.Aggregates
{
    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }

        public decimal DailyLimit { get; set; }
        public decimal WithdrawnToday { get; set; }
    }
}
