using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace ConsoleBankDataAccess
{
    public class DataAccess
    {
        private SqlConnection _connection = null;
        public string connectionString = ConfigurationManager.ConnectionStrings["ConsoleBankingSqlServer"].ConnectionString;
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
        public void CreateCustomerAccount(AccountModel newCustomer)
        {
            OpenConnection();
            string sql = "INSERT INTO CUSTOMER" + "(FirstName,LastName,Email,AccountNumber,UserName,Password,AccountType,AccountBalance,DateCreated, TimeCreated,Pin)VALUES" + $"('{newCustomer.FirstName}','{newCustomer.LastName}','{newCustomer.Email}','{newCustomer.AccountNumber}','{newCustomer.UserName}','{newCustomer.Password}','{newCustomer.AccountType}','{newCustomer.Balance}',@dateCreated,@timeCreated,{newCustomer.Pin})";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.Parameters.Add("@dateCreated", SqlDbType.Date).Value = DateTime.Now.ToShortDateString();
            command.Parameters.Add("@timeCreated", SqlDbType.Time).Value = DateTime.Now.TimeOfDay.ToString();
            command.ExecuteNonQuery();
        }
        // Read data using either  user's Username.
        public bool ReadFromCustomerWithUsername(AccountModel customer)
        {
            OpenConnection();
            bool userfound = false;
            string sql = $"Select * From Customer Where Username = '{customer.UserName}'";

            using (SqlCommand command = new SqlCommand(sql, _connection))
            {
                using SqlDataReader readCustomer = command.ExecuteReader();

                if (readCustomer.Read())
                {
                    customer.FirstName = (string)readCustomer["FirstName"];
                    customer.LastName = (string)readCustomer["LastName"];
                    customer.Email = (string)readCustomer["Email"];
                    customer.AccountNumber = (decimal)readCustomer["AccountNumber"];
                    customer.UserName = (string)readCustomer["UserName"];
                    customer.Password = (string)readCustomer["Password"];
                    customer.AccountType = (AccountType)Enum.Parse(typeof(AccountType), "" + readCustomer["AccountType"]);
                    customer.Balance = (decimal)readCustomer["AccountBalance"];
                    customer.DateCreated = (DateTime)readCustomer["DateCreated"];
                    customer.TimeCreated = (TimeSpan)readCustomer["TimeCreated"];
                    customer.Pin = (int)readCustomer["Pin"];
                    userfound = true;
                }
                else
                {
                    userfound = false;
                }
            }
            return userfound;
        }
        // Read data using user's pin
        public bool ReadFromCustomerWithPin(AccountModel customer)
        {
            OpenConnection();
            bool pinfound = false;
            string sql = $"Select * From Customer Where Pin = '{customer.Pin}'";

            using (SqlCommand command = new SqlCommand(sql, _connection))
            {
                using SqlDataReader readCustomer = command.ExecuteReader();

                if (readCustomer.Read())
                {
                    customer.FirstName = (string)readCustomer["FirstName"];
                    customer.LastName = (string)readCustomer["LastName"];
                    customer.Email = (string)readCustomer["Email"];
                    customer.AccountNumber = (decimal)readCustomer["AccountNumber"];
                    customer.UserName = (string)readCustomer["UserName"];
                    customer.Password = (string)readCustomer["Password"];
                    customer.AccountType = (AccountType)Enum.Parse(typeof(AccountType), "" + readCustomer["AccountType"]);
                    customer.Balance = (decimal)readCustomer["AccountBalance"];
                    customer.DateCreated = (DateTime)readCustomer["DateCreated"];
                    customer.TimeCreated = (TimeSpan)readCustomer["TimeCreated"];
                    customer.Pin = (int)readCustomer["Pin"];
                    pinfound = true;
                }
                else
                {
                    pinfound = false;
                }
            }
            return pinfound;
        }
        // Create a new transaction
        public void CreateTransaction(TransactionModel trans, string username)
        {
            OpenConnection();
            string sql = "INSERT INTO TRANSACTIONS" + "(UserName,Amount,Description,Type,Date,Time,Status)VALUES" + $"('{username}',{trans.TransactionAmount},'{trans.TransactionDescription}','{trans.TransactionType}',@dateOfTrans,@timeOfTrans,'{trans.TransactionStatus}')";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.Parameters.Add("@dateOfTrans", SqlDbType.Date).Value = DateTime.Now.ToShortDateString();
            command.Parameters.Add("@timeOfTrans", SqlDbType.Time).Value = DateTime.Now.TimeOfDay.ToString();
            command.ExecuteNonQuery();
        }
        // View transaction history as table
        public DataTable ReadFromTransactionAsTable(string username)
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
                using SqlDataReader getUsername = command.ExecuteReader();

                while (getUsername.Read())
                {
                    for (int i = 0; i < getUsername.FieldCount; i++)
                    {
                        if (username == (string)getUsername.GetValue(i))
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
