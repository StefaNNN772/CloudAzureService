using HealthMonitoringContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class HealthMonitoringProvider : IHealthMonitoring
    {
        public bool CheckServices()
        {
            return true;
        }
    }
}
