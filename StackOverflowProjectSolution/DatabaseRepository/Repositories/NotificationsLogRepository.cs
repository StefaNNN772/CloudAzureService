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
using DatabaseRepository.Models;

namespace DatabaseRepository.Repositories
{
    public class NotificationsLogRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public NotificationsLogRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(
                new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("NotificationsLogTable");
            _table.CreateIfNotExists();
        }

        public IQueryable<NotificationsLog> RetrieveAllNotificationLogs()
        {
            var results = from g in _table.CreateQuery<NotificationsLog>()
                          where g.PartitionKey == "NotificationsLog"
                          select g;
            return results;
        }

        public void addLogsNotification(NotificationsLog log)
        {
            TableOperation insertOperation = TableOperation.Insert(log);
            _table.Execute(insertOperation);
        }

        public NotificationsLog GetLogByRowKey(string rowKey)
        {
            var result = (from g in _table.CreateQuery<NotificationsLog>()
                          where g.PartitionKey == "NotificationsLog" && g.RowKey == rowKey
                          select g).FirstOrDefault();
            return result;
        }

        public void DeleteNotificationsLog(string rowKey)
        {
            try
            {
                var log = GetLogByRowKey(rowKey);
                TableOperation deleteOperation = TableOperation.Delete(log);
                _table.Execute(deleteOperation);
            }
            catch (StorageException ex)
            {
                throw new Exception("Error deleting log: " + ex.Message, ex);
            }
        }
        
    }
}
