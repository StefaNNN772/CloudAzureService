using HealthMonitoringContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowService
{
    public class HealthManagerProvider : IHealthMonitoring
    {
        public bool CheckServices()
        {
            return true;
        }
    }
}