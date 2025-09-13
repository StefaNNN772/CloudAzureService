using AdminToolsConsoleApp.UniversalConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoringContracts;
using AdminToolsConsoleApp.HelperMethods;

namespace AdminToolsConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Connect to the service
            ServiceConnector<IAdminAlertEmails> serviceConnector = new ServiceConnector<IAdminAlertEmails>();
            serviceConnector.Connect("net.tcp://localhost:10102/AdminAlertEmails");
            IAdminAlertEmails emailService = serviceConnector.GetProxy();

            // Execute commands
            Executor executor = new Executor();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("1. Add Email");
                Console.WriteLine("2. List All Emails");
                Console.WriteLine("3. Remove Email");
                Console.WriteLine("4. Exit");

                Console.Write("\nEnter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        executor.AddEmail(emailService);
                        break;
                    case "2":
                        executor.ListAllEmails(emailService);
                        break;
                    case "3":
                        executor.RemoveEmail(emailService);
                        break;
                    case "4":
                        exit = true;
                        break;
                  
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}
