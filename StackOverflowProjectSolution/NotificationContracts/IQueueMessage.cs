using System;
using System.Collections.Generic;

namespace NotificationContracts
{
    public interface IQueueMessage
    {
        string MessageType { get; set; }
        Dictionary<string, object> Data { get; set; }
        List<string> EmailAddresses { get; set; }
        DateTime Timestamp { get; set; }
    }
}