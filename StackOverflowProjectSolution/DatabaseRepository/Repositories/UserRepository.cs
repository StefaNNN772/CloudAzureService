using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using System.Collections;

namespace DatabaseRepository.Repositories
{
    public class UserRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public UserRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(
                new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<User> RetrieveAllUsers()
        {
            var results = from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User"
                          select g;
            return results;
        }

        public User GetUser(string email)
        {
            var result = (from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User" && g.Email == email
                          select g).FirstOrDefault();
            return result;
        }

        public User GetUserByRowKey(string rowKey)
        {
            var result = (from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User" && g.RowKey == rowKey
                          select g).FirstOrDefault();
            return result;
        }

        public bool UserExists(string email)
        {
            var query = (from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User" && g.Email == email
                          select g).Take(1);

            var result = query.FirstOrDefault();
            return result != null;
        }

        public bool UserExistsLogin(string email, string password)
        {
            var query = (from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User" && g.Email == email
                          select g).Take(1);

            var result = query.FirstOrDefault();

            if (result != null)
            {
                return BCrypt.Net.BCrypt.Verify(password, result.Password);
            }
            return false;
        }
        public void AddUser(User user)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            TableOperation insertOperation = TableOperation.Insert(user);
            _table.Execute(insertOperation);
        }

        public void UpdateUser(User user)
        {
            try
            {
                TableOperation updateOperation = TableOperation.Replace(user);
                _table.Execute(updateOperation);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == 412)
                {
                    System.Diagnostics.Debug.WriteLine($"Concurrent update detected for user {user.Email}");
                    throw new Exception("Someone else modified this user. Please refresh and try again.", ex);
                }

                throw new Exception("Error updating user: " + ex.Message, ex);
            }
        }
    }
}
