using DatabaseRepository.Models;
using DatabaseRepository.Repositories;
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

        private static AlertEmailsRepository repo = new AlertEmailsRepository();

        public bool AddEmail(string email)
        {
            AlertEmails alertEmail = new AlertEmails(email) { Email = email};
            bool result = repo.AddAlertEmail(alertEmail);
            
            return result;
        }

        public List<string> GetAllEmails()
        {
            List<string> result = new List<string>();
            var retrieve = repo.GetAllAlertEmails();
            foreach(var ret in retrieve)
            {
                result.Add(ret.Email);
            }

            return result;
        }

        public bool RemoveEmail(string email)
        {
            AlertEmails alertEmail = new AlertEmails(email);
            bool result = repo.RemoveAlertEmail(alertEmail);

            return result;
        }
    }
}
