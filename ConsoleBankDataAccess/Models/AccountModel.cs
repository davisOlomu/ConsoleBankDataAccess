using System;

namespace ConsoleBankDataAccess
{
    public class AccountModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        public TimeSpan TimeCreated { get; set; }
        public decimal AccountNumber { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public int Pin { get; set; }

    }
}
