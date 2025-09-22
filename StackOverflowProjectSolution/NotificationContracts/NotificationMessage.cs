using System;
using System.Collections.Generic;

namespace NotificationContracts
{
    public class NotificationMessage : IQueueMessage
    {
        public string MessageType { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public List<string> EmailAddresses { get; set; }
        public DateTime Timestamp { get; set; }

        public NotificationMessage()
        {
            Data = new Dictionary<string, object>();
            EmailAddresses = new List<string>();
            Timestamp = DateTime.UtcNow;
        }
    }

    public static class MessageTypes
    {
        public const string BestAnswerSelected = "BestAnswerSelected";
        public const string ServiceHealthAlert = "ServiceHealthAlert";
    }
}