using DatabaseRepository.Models;
using DatabaseRepository.Repositories;
using HealthMonitoringContracts;
using HealthMonitoringService.UniversalConnector;
using NotificationContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace HealthMonitoringService
{
    //public class JobServerProvider :  IHealthMonitoring
    //{
    //    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    //    private readonly HealthCheckRepository _healthCheckRepository = new HealthCheckRepository();
    //    private readonly AlertEmailsRepository _alertEmailsRepository = new AlertEmailsRepository();
    //    private NotifyAlertEmailsPartialServer notifyAlertEmailsPartialServer = new NotifyAlertEmailsPartialServer();


    //    public bool CheckServices()
    //    {
    //        _ = Task.Run(async () =>
    //        {
    //            while (!cancellationTokenSource.Token.IsCancellationRequested)
    //            {

    //                    ServiceConnector<IHealthMonitoring> serviceConnector = new ServiceConnector<IHealthMonitoring>();
    //                    serviceConnector.Connect("net.tcp://localhost:10105/HealthMonitoring");
    //                    IHealthMonitoring healthMonitoringStackOverflowService = serviceConnector.GetProxy();


    //                    ServiceConnector<IHealthMonitoring> serviceNotificationConnector = new ServiceConnector<IHealthMonitoring>();
    //                    serviceNotificationConnector.Connect("net.tcp://localhost:10103/HealthMonitoring");
    //                    IHealthMonitoring healthMonitoringNotificationService = serviceNotificationConnector.GetProxy();

               

    //                List<AlertEmails> emails = _alertEmailsRepository.GetAllAlertEmails().ToList();
    //                List<string> emailsString = new List<string>();

    //                foreach (var email in emails)
    //                {
    //                    emailsString.Add(email.Email);
    //                }

    //                try
    //                {
    //                    bool isAliveSO = healthMonitoringStackOverflowService.CheckServices();
    //                    Trace.TraceInformation(isAliveSO ? "StackOverflowService ok" : "StackOverflowService not_ok");

    //                    string rowKey = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    //                    HealthCheck healthCheckSO = new HealthCheck(rowKey);

    //                    healthCheckSO.Date = DateTime.UtcNow;
    //                    healthCheckSO.ServiceName = "StackOverflowService";
    //                    healthCheckSO.Status = isAliveSO ? "ok" : "not_ok";

    //                    _healthCheckRepository.AddHealthCheck(healthCheckSO);

    //                    bool isAliveNotification = healthMonitoringNotificationService.CheckServices();
    //                    Trace.TraceInformation(isAliveNotification ? "NotificationService ok" : "NotificationService not_ok");

    //                    rowKey = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    //                    HealthCheck healthCheckNO = new HealthCheck(rowKey);

    //                    healthCheckNO.Date = DateTime.UtcNow;
    //                    healthCheckNO.ServiceName = "NotificationService";
    //                    healthCheckNO.Status = isAliveNotification ? "ok" : "not_ok";

    //                    _healthCheckRepository.AddHealthCheck(healthCheckNO);

                    

    //                }
    //                catch (Exception ex)
    //                {
    //                    string message = $"not_ok - {ex.Message}";
    //                    Trace.TraceError(message);
                       
                   
    //                }

    //                        await Task.Delay(TimeSpan.FromSeconds(4), cancellationTokenSource.Token);
    //            }
    //        }, cancellationTokenSource.Token);

    //        notifyAlertEmailsPartialServer.Close();
    //        return true;

    //            }

       
    //}
}
