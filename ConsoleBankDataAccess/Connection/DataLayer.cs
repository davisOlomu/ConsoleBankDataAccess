using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ConsoleBankDataAccess
{
    public class DataLayer
    {
        private SqlConnection _connection = null;
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConsoleBankingSqlServer"].ConnectionString;

        
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
        // New user account
        public void CreateAccount(AccountModel newAccount)
        {
            OpenConnection();
            string sql = "INSERT INTO CUSTOMER" + "(FirstName,LastName,Email,AccountNumber,UserName,Password,AccountType,AccountBalance,DateCreated, TimeCreated,Pin)VALUES" + $"('{newAccount.FirstName}','{newAccount.LastName}','{newAccount.Email}','{newAccount.AccountNumber}','{newAccount.UserName}','{newAccount.Password}','{newAccount.AccountType}','{newAccount.Balance}',@dateCreated,@timeCreated,{newAccount.Pin})";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.Parameters.Add("@dateCreated", SqlDbType.Date).Value = DateTime.Now.ToShortDateString();
            command.Parameters.Add("@timeCreated", SqlDbType.Time).Value = DateTime.Now.TimeOfDay.ToString();
            command.ExecuteNonQuery();
        }
        // Read user's data
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
        // Create a new transaction
        public void CreateTransaction(TransactionModel newTransaction, string username)
        {
            OpenConnection();
            string sql = "INSERT INTO TRANSACTIONS" + "(UserName,Amount,Description,Type,Date,Time,Status)VALUES" + $"('{username}',{newTransaction.TransactionAmount},'{newTransaction.TransactionDescription}','{newTransaction.TransactionType}',@dateOfTrans,@timeOfTrans,'{newTransaction.TransactionStatus}')";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.Parameters.Add("@dateOfTrans", SqlDbType.Date).Value = DateTime.Now.ToShortDateString();
            command.Parameters.Add("@timeOfTrans", SqlDbType.Time).Value = DateTime.Now.TimeOfDay.ToString();
            command.ExecuteNonQuery();
        }
        // View transaction history as table
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
        // Update balance.
        public void UpdateBalance(AccountModel account, decimal balance)
        {
            OpenConnection();
            string sql = $"Update CUSTOMER Set AccountBalance = {balance} Where Username = '{account.UserName}'";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.ExecuteNonQuery();
        }
        // Username has to be unique.
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
