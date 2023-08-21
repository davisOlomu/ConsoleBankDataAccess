using System;

namespace ConsoleBankDataAccess
{
    /// <summary>
    /// Model the Transaction table.
    /// </summary>
    public class TransactionModel
    {
        public string Username { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionDescription { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionTime { get; set; }
        public TransactionStatus TransactionStatus = new Int32();
        public TransactionType TransactionType = new Int32();
    }
}
