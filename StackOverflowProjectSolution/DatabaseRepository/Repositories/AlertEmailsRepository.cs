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
        public IQueryable<AlertEmails> GetAllAlertEmails()
        {
            var results = from g in _table.CreateQuery<AlertEmails>()
                          where g.PartitionKey == "AlertEmails"
                          select g;
            return results;
        }
        public bool AddAlertEmail(AlertEmails alertEmail)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            var entity = _table.CreateQuery<AlertEmails>()
                     .Where(g => g.PartitionKey == "AlertEmails"
                              && g.RowKey == alertEmail.RowKey)
                     .FirstOrDefault();

            if (entity != null)
                return false;

            TableOperation insertOperation = TableOperation.Insert(alertEmail);
            var result = _table.Execute(insertOperation);
            return result != null && result.HttpStatusCode == 204;
        }

        public  bool  RemoveAlertEmail(AlertEmails alertEmail)
        {
            var entity = _table.CreateQuery<AlertEmails>()
                      .Where(g => g.PartitionKey == "AlertEmails"
                               && g.RowKey == alertEmail.RowKey)
                      .FirstOrDefault();

            if (entity == null)
                return false; // nothing to delete




            TableOperation deleteOperation = TableOperation.Delete(entity);
            var result = _table.Execute(deleteOperation);
            return result != null && result.HttpStatusCode == 204;
        }
    }
}
