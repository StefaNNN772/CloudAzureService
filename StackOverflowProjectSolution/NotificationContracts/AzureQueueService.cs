using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace NotificationContracts
{
    public class AzureQueueService
    {
        private CloudQueue _notificationQueue;
        private const string DefaultQueueName = "notifications";

        public AzureQueueService(string queueName = DefaultQueueName)
        {
            InitializeQueue(queueName);
        }

        private void InitializeQueue(string queueName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("DataConnectionString"));
                
                var queueClient = storageAccount.CreateCloudQueueClient();
                _notificationQueue = queueClient.GetQueueReference(queueName);
                _notificationQueue.CreateIfNotExists();
                
                System.Diagnostics.Trace.TraceInformation($"Azure Queue '{queueName}' initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Failed to initialize Azure Queue '{queueName}': {ex.Message}");
                throw new Exception($"Failed to initialize Azure Queue: {ex.Message}", ex);
            }
        }

        public async Task SendMessageAsync(NotificationMessage message)
        {
            try
            {
                var messageJson = JsonConvert.SerializeObject(message);
                var queueMessage = new CloudQueueMessage(messageJson);
                
                await _notificationQueue.AddMessageAsync(queueMessage);
                
                System.Diagnostics.Trace.TraceInformation($"Message sent to queue: Type={message.MessageType}, Recipients={message.EmailAddresses?.Count ?? 0}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Failed to send message to queue: {ex.Message}");
                throw new Exception($"Failed to send message to queue: {ex.Message}", ex);
            }
        }

        public async Task<NotificationMessage> ReceiveMessageAsync()
        {
            try
            {
                var message = await _notificationQueue.GetMessageAsync();
                
                if (message == null)
                    return null;

                var notificationMessage = JsonConvert.DeserializeObject<NotificationMessage>(message.AsString);
                
                // Delete the message from queue after successful processing
                await _notificationQueue.DeleteMessageAsync(message);
                
                return notificationMessage;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to receive message from queue: {ex.Message}", ex);
            }
        }

        public async Task<int> GetApproximateMessageCountAsync()
        {
            try
            {
                await _notificationQueue.FetchAttributesAsync();
                return _notificationQueue.ApproximateMessageCount ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get message count: {ex.Message}", ex);
            }
        }
    }
}