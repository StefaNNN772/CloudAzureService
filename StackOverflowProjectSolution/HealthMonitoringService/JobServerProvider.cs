using HealthMonitoringContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class JobServerProvider : IAdminAlertEmails, IHealthMonitoring
    {
        public bool AddEmail(string email)
        {
            throw new NotImplementedException();
        }

        public bool CheckServices()
        {
            throw new NotImplementedException();
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
