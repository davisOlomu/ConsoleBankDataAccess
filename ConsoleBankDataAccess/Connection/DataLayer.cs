﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ConsoleBankDataAccess
{
    public class DataLayer
    {
        private SqlConnection _connection = null;
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConsoleBankingSqlServer"].ConnectionString;

        /// <summary>
        /// Create a connection using the connectionstring
        /// located in the app.config file.
        /// </summary>
        public void OpenConnection()
        {
            _connection = new SqlConnection
            {
                ConnectionString = connectionString
            };
            try
            {
                _connection.Open();
            }
            catch (Exception)
            {
                Console.WriteLine("There is an error while establishing a connection with the SqlServer");
            }
        }

        /// <summary>
        /// User data inserted into the Customer table.
        /// Entrys's are validated and must meet certain requirements.
        /// </summary>
        /// <param name="newAccount">A new user account.</param>
        public void CreateAccount(AccountModel newAccount)
        {
            OpenConnection();
            string sql = "INSERT INTO CUSTOMER" + "(FirstName,LastName,Email,AccountNumber,UserName,Password,AccountType,AccountBalance,DateCreated, TimeCreated,Pin)VALUES" + $"('{newAccount.FirstName}','{newAccount.LastName}','{newAccount.Email}','{newAccount.AccountNumber}','{newAccount.UserName}','{newAccount.Password}','{newAccount.AccountType}','{newAccount.Balance}',@dateCreated,@timeCreated,{newAccount.Pin})";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.Parameters.Add("@dateCreated", SqlDbType.Date).Value = DateTime.Now.ToShortDateString();
            command.Parameters.Add("@timeCreated", SqlDbType.Time).Value = DateTime.Now.TimeOfDay.ToString();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Reads a valid user's information from the Customer table.
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="sql">Sql Select Statement</param>
        /// <returns></returns>
        public bool GetUser(AccountModel user, string sql)
        {
            OpenConnection();

            bool isUser = false;
            using (SqlCommand command = new SqlCommand(sql, _connection))
            {
                using SqlDataReader readUser = command.ExecuteReader();

                if (readUser.Read())
                {
                    user.FirstName = (string)readUser["FirstName"];
                    user.LastName = (string)readUser["LastName"];
                    user.Email = (string)readUser["Email"];
                    user.AccountNumber = (decimal)readUser["AccountNumber"];
                    user.UserName = (string)readUser["UserName"];
                    user.Password = (string)readUser["Password"];
                    user.AccountType = (AccountType)Enum.Parse(typeof(AccountType), "" + readUser["AccountType"]);
                    user.Balance = (decimal)readUser["AccountBalance"];
                    user.DateCreated = (DateTime)readUser["DateCreated"];
                    user.TimeCreated = (TimeSpan)readUser["TimeCreated"];
                    user.Pin = (int)readUser["Pin"];
                    isUser = true;
                }
                else
                {
                    isUser = false;
                }
            }
            return isUser;
        }

        /// <summary>
        /// A new transaction is inserted into the Transaction table.
        /// </summary>
        /// <param name="newTransaction">A new transaction</param>
        /// <param name="username">Transaction owner</param>
        public void CreateTransaction(TransactionModel newTransaction, string username)
        {
            OpenConnection();
            string sql = "INSERT INTO TRANSACTIONS" + "(UserName,Amount,Description,Type,Date,Time,Status)VALUES" + $"('{username}',{newTransaction.TransactionAmount},'{newTransaction.TransactionDescription}','{newTransaction.TransactionType}',@dateOfTrans,@timeOfTrans,'{newTransaction.TransactionStatus}')";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.Parameters.Add("@dateOfTrans", SqlDbType.Date).Value = DateTime.Now.ToShortDateString();
            command.Parameters.Add("@timeOfTrans", SqlDbType.Time).Value = DateTime.Now.TimeOfDay.ToString();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Reads a valid user's transactions
        /// </summary>
        /// <param name="username">Transaction owner</param>
        /// <returns></returns>
        public DataTable GetTransactionHistory(string username)
        {
            OpenConnection();
            TransactionModel transModel = new TransactionModel();
            DataTable table = new DataTable();
            string sql = $"Select Amount,Type,Status,Time,Description From Transactions Where Username = '{username}'";

            using (SqlCommand command = new SqlCommand(sql, _connection))
            {
                using SqlDataReader getTrans = command.ExecuteReader();
                table.Load(getTrans);
                getTrans.Close();
            }
            return table;
        }

        /// <summary>
        /// Ensures user balance is consistent with every transaction.
        /// </summary>
        /// <param name="account">Transaction account</param>
        /// <param name="balance">User balance</param>
        public void UpdateBalance(AccountModel account, decimal balance)
        {
            OpenConnection();
            string sql = $"Update CUSTOMER Set AccountBalance = {balance} Where Username = '{account.UserName}'";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Ensures no two users have the same username at the time
        /// of creating a new account.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool VerifyUserName(string username)
        {
            OpenConnection();
            string sql = "Select UserName From CUSTOMER";
            bool usernameExist = false;

            using (SqlCommand command = new SqlCommand(sql, _connection))
            {
                using SqlDataReader readUsername = command.ExecuteReader();

                while (readUsername.Read())
                {
                    for (int i = 0; i < readUsername.FieldCount; i++)
                    {
                        if (username == (string)readUsername.GetValue(i))
                        {
                            usernameExist = true;
                        }
                    }
                }
            }
            return usernameExist;
        }
    }
}
