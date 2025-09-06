using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NotificationContracts
{
    [ServiceContract]
    public interface INotifyAlertEmails
    {
        [OperationContract]
        Task SendEmailsAsync(List<string> emails, string emailBody);

    }
}
