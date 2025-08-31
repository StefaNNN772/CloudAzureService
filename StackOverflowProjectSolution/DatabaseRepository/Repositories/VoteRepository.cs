using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseRepository.Models;

namespace DatabaseRepository.Repositories
{
    public class VoteRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public VoteRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(
                new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("VoteTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<Vote> RetrieveAllVotes()
        {
            var results = from g in _table.CreateQuery<Vote>()
                          where g.PartitionKey == "Vote"
                          select g;
            return results;
        }
        public void AddVote(Vote vote)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            TableOperation insertOperation = TableOperation.Insert(vote);
            _table.Execute(insertOperation);
        }
    }
}
