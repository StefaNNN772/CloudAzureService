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
    public class AlertEmailsRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public AlertEmailsRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AlertEmailsConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(
                new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("AlertEmailsTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<AlertEmails> RetrieveAllAlertEmails()
        {
            var results = from g in _table.CreateQuery<AlertEmails>()
                          where g.PartitionKey == "AlertEmails"
                          select g;
            return results;
        }
        public void AddAlertEmail(AlertEmails alertEmails)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            TableOperation insertOperation = TableOperation.Insert(alertEmails);
            _table.Execute(insertOperation);
        }
    }
}
