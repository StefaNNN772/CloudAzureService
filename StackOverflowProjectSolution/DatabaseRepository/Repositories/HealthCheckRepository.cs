using DatabaseRepository.Models;
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
    public class HealthCheckRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public HealthCheckRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("HealthCheckConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(
                new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("HealthCheckTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<HealthCheck> RetrieveAllHealthChecks()
        {
            DateTime threshold = DateTime.UtcNow.AddHours(-3);

            var results = from g in _table.CreateQuery<HealthCheck>()
                          where g.PartitionKey == "HealthCheck"
                          && g.Date >= threshold
                          select g;
            return results.ToList().AsQueryable(); // izvrši query u memoriji
        }
        public void AddHealthCheck(HealthCheck healthCheck)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            TableOperation insertOperation = TableOperation.Insert(healthCheck);
            _table.Execute(insertOperation);
        }
    }
}
