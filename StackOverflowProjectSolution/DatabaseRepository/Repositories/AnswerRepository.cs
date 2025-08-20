using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository.Repositories
{
    public class AnswerRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public AnswerRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(
                new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("AnswerTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<Answer> RetrieveAllAnswers()
        {
            var results = from g in _table.CreateQuery<Answer>()
                          where g.PartitionKey == "Answer"
                          select g;
            return results;
        }
        public void AddAnswer(Answer answer)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            TableOperation insertOperation = TableOperation.Insert(answer);
            _table.Execute(insertOperation);
        }
    }
}
