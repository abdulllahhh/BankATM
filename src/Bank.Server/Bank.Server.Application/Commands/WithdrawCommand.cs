using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Application.Commands
{
    public class WithdrawCommand
    {
        public string CardNumber { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
