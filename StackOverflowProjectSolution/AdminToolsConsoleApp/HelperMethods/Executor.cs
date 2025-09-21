
using HealthMonitoringContracts;
using System;
using System.Collections.Generic;

namespace AdminToolsConsoleApp.HelperMethods
{
    public class Executor : IExecutor
    {
       
        public void AddEmail(IAdminAlertEmails emailService)
        {
            Console.WriteLine();
            Console.Write("Enter email: ");
            string email = Console.ReadLine();
            

            

            Console.WriteLine();
            if (emailService.AddEmail(email))
            {
                Console.WriteLine("Email has been added successfully.");
            }
            else
            {
                Console.WriteLine("Error adding email!");
            }
        }

        public void ListAllEmails(IAdminAlertEmails emailService)
        {
            List<string> emails = emailService.GetAllEmails();
            Console.WriteLine("\n========================== ALL ADMINS =========================== ");
            foreach (var email in emails)
            {
                Console.WriteLine($"Email: {email}");
            }
            Console.WriteLine("===================================================================");
        }

        public void RemoveEmail(IAdminAlertEmails emailService)
        {
            Console.Write("\n Enter email to remove: ");
            string email = Console.ReadLine();

            Console.WriteLine();
            if (emailService.RemoveEmail(email))
            {
                Console.WriteLine("Email has been removed.");
            }
            else
            {
                Console.WriteLine("Entered email  doesn't exist!");
            }
        }
    }
}
