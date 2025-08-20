using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringContracts
{
    [ServiceContract]
    public interface IAdminAlertEmails
    {

        [OperationContract]
        bool AddEmail(string email);

        [OperationContract]
        List<string> GetAllEmails();

        [OperationContract]
        bool RemoveEmail(string email);

    }
}
