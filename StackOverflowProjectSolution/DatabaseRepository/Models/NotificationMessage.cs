using System;

namespace DatabaseRepository.Models
{
    public class NotificationMessage
    {
        public string AnswerId { get; set; }
        public string MessageType { get; set; } // "BestAnswerSelected" or "HealthAlert"
        public DateTime CreatedAt { get; set; }
        public string AdditionalData { get; set; } // For any extra data needed
        
        public NotificationMessage()
        {
            CreatedAt = DateTime.UtcNow;
        }
        
        public NotificationMessage(string answerId, string messageType = "BestAnswerSelected") : this()
        {
            AnswerId = answerId;
            MessageType = messageType;
        }
    }
}