using HealthMonitoringContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class AdminAlertEmailsProvider : IAdminAlertEmails
    {
        public bool AddEmail(string email)
        {
            Console.WriteLine($"Added email: {email}.");
            return true;
        }

        public List<string> GetAllEmails()
        {
            throw new NotImplementedException();
        }

        public bool RemoveEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}
