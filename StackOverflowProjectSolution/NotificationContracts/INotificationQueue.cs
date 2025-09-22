using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NotificationContracts
{
    [ServiceContract]
    public interface INotificationQueue
    {
        [OperationContract]
        Task SendToQueueAsync(NotificationMessage message);

        [OperationContract]
        Task<NotificationMessage> ReceiveFromQueueAsync();
    }
}