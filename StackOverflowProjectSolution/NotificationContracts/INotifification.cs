using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NotificationContracts
{
    [ServiceContract]
    public interface IJob
    {
        [OperationContract]
        void SendEmails(List<string> emails, string emailBody);

    }
}
