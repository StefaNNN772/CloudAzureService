using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

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
        public void AddUser(User user)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            TableOperation insertOperation = TableOperation.Insert(user);
            _table.Execute(insertOperation);
        }
    }
}
