using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DatabaseRepository.Helpers
{
    public class QueueHelper
    {
        private CloudStorageAccount _storageAccount;
        private CloudQueueClient _queueClient;

        public QueueHelper(string connectionStringName = "DataConnectionString")
        {
            _storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting(connectionStringName));
            _queueClient = _storageAccount.CreateCloudQueueClient();
        }

        public async Task<T> ReceiveMessageAsync<T>(string queueName, TimeSpan? visibilityTimeout = null) where T : class
        {
            try
            {
                var message = await ReceiveMessageAsync(queueName, visibilityTimeout);
                if (message == null) return null;
                
                return JsonConvert.DeserializeObject<T>(message.AsString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to deserialize message from queue {queueName}: {ex.Message}");
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

        public async Task<CloudQueueMessage> ReceiveMessageAsync(string queueName, TimeSpan? visibilityTimeout = null)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                await queue.CreateIfNotExistsAsync();
                
                return await queue.GetMessageAsync(visibilityTimeout);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to receive message from queue {queueName}: {ex.Message}");
            }
        }

        public async Task DeleteMessageAsync(string queueName, CloudQueueMessage message)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                await queue.DeleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete message from queue {queueName}: {ex.Message}");
            }
        }
    }
}