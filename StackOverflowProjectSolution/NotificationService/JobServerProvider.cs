using NotificationContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class JobServerProvider : INotifification
    {
        public void SendEmails(List<string> emails, string emailBody)
        {
            throw new NotImplementedException();
        }
    }
}
