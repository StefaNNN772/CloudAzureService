using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using DatabaseRepository.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace StackOverflowService.Helpers
{
    public class QueueHelper
    {
        private CloudStorageAccount _storageAccount;
        private CloudQueueClient _queueClient;

        public QueueHelper()
        {
            _storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("DataConnectionString"));
            _queueClient = _storageAccount.CreateCloudQueueClient();
        }

        public async Task SendNotificationMessageAsync(string queueName, NotificationMessage notificationMessage)
        {
            try
            {
                string jsonMessage = JsonConvert.SerializeObject(notificationMessage);
                await SendMessageAsync(queueName, jsonMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send notification message to queue {queueName}: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                await queue.CreateIfNotExistsAsync();
                
                CloudQueueMessage queueMessage = new CloudQueueMessage(message);
                await queue.AddMessageAsync(queueMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send message to queue {queueName}: {ex.Message}");
            }
        }
    }
}