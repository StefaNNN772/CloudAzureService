using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoringService.UniversalConnector;
using HealthMonitoringContracts;
using DatabaseRepository.Repositories;
using DatabaseRepository.Models;
using DatabaseRepository.Helpers;
using NotificationContracts;
using System.ServiceModel;

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly HealthCheckRepository _healthCheckRepository = new HealthCheckRepository();
        private readonly AlertEmailsRepository _alertEmailsRepository = new AlertEmailsRepository();

        private NotifyAlertEmailsPartialServer notifyAlertEmails = new NotifyAlertEmailsPartialServer();
        //private JobServer jobServer = new JobServer();
        private AdminAlertEmailsServer aaeServer = new AdminAlertEmailsServer();

        private bool isAliveSO ;
        private bool isAliveNO ;
        
        public override void Run()
        {
            Trace.TraceInformation("HealthMonitoringService is running");

            try
            {
                // Ako hoćeš u Event Viewer
                if (!EventLog.SourceExists("HealthMonitoringService"))
                {
                    EventLog.CreateEventSource("HealthMonitoringService", "Application");
                }
                Trace.Listeners.Add(new EventLogTraceListener("HealthMonitoringService"));
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Use TLS 1.2 for Service Bus connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            //jobServer.Open();
            aaeServer.Open();
            notifyAlertEmails.Open();

            Trace.TraceInformation("HealthMonitoringService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            //jobServer.Close();
            aaeServer.Close();
            notifyAlertEmails.Close();

            Trace.TraceInformation("HealthMonitoringService has stopped");
        }

        private List<EndpointAddress> ConnectToInternalServices(NetTcpBinding binding,string internalEndpointName)
        {
            
            var currentRoleInstanceId = RoleEnvironment.CurrentRoleInstance.Id;
            var internalEndpoints = new List<EndpointAddress>();

            foreach(var roleInstance in RoleEnvironment.Roles[RoleEnvironment.CurrentRoleInstance.Role.Name].Instances)
            {
                if(currentRoleInstanceId != roleInstance.Id)
                {
                    internalEndpoints.Add(new EndpointAddress(string.Format("net.tcp://{0}/{1}", roleInstance.InstanceEndpoints[internalEndpointName].IPEndpoint.ToString(), internalEndpointName)));
                }
            }
            return internalEndpoints;
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.

            ServiceConnector<IHealthMonitoring> serviceConnector = new ServiceConnector<IHealthMonitoring>();
            serviceConnector.Connect("net.tcp://localhost:10105/HealthMonitoring");
            IHealthMonitoring healthMonitoringStackOverflowService = serviceConnector.GetProxy();


            ServiceConnector<IHealthMonitoring> serviceNotificationConnector = new ServiceConnector<IHealthMonitoring>();
            serviceNotificationConnector.Connect("net.tcp://localhost:10103/HealthMonitoring");
            IHealthMonitoring healthMonitoringNotificationService = serviceNotificationConnector.GetProxy();

            NetTcpBinding binding = new NetTcpBinding();
            List<EndpointAddress> internalEndpoints = ConnectToInternalServices(binding,"SendAlertEmails");
            INotifyAlertEmails sendEmailsProxy = (new ChannelFactory<INotifyAlertEmails>(binding, internalEndpoints[0])).CreateChannel();


            List<AlertEmails> emails = _alertEmailsRepository.GetAllAlertEmails().ToList();
            List<string> emailsString = new List<string>();
            foreach(var email in emails)
            {
                emailsString.Add(email.Email);
            }
           

            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {

                        isAliveSO = healthMonitoringStackOverflowService.CheckServices();
                        Trace.TraceInformation(isAliveSO ? "StackOverflowService ok" : "StackOverflowService not_ok");

                        DateTime utcNow = DateTime.UtcNow;
                        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);

                        string rowKey = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                        HealthCheck healthCheckSO = new HealthCheck(rowKey);

                        healthCheckSO.Date = DateTime.Now;
                        healthCheckSO.ServiceName = "StackOverflowService";
                        healthCheckSO.Status = isAliveSO ? "ok" : "not_ok";

                        _healthCheckRepository.AddHealthCheck(healthCheckSO);

                         isAliveNO = healthMonitoringNotificationService.CheckServices();
                        Trace.TraceInformation(isAliveNO ? "NotificationService ok" : "NotificationService not_ok");

                        rowKey = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                        HealthCheck healthCheckNO = new HealthCheck(rowKey);

                        healthCheckNO.Date = DateTime.Now;
                        healthCheckNO.ServiceName = "NotificationService";
                        healthCheckNO.Status = isAliveNO ? "ok" : "not_ok";

                        _healthCheckRepository.AddHealthCheck(healthCheckNO);

                        

                    }
                    catch (Exception ex)
                    {

                        string rowKey = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                        HealthCheck healthCheck = new HealthCheck(rowKey);

                        DateTime utcNow = DateTime.UtcNow;
                        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);


                        if (!isAliveSO)
                        {
                            Trace.TraceInformation("StackOverflowService not_ok");


                            healthCheck.Date = DateTime.Now;
                            healthCheck.ServiceName = "StackOverflowService";
                            healthCheck.Status = "not_ok";

                        }
                        else
                        {
                            Trace.TraceInformation( "NotificationService not_ok");

                            healthCheck.Date = DateTime.Now;
                            healthCheck.ServiceName = "NotificationService";
                            healthCheck.Status =  "not_ok";
                        }
                         _healthCheckRepository.AddHealthCheck(healthCheck);

                        string message = $"not_ok - {ex.Message}";
                        Trace.TraceError(message);

                        // Send health alert message to queue instead of direct service call
                        var queueHelper = new QueueHelper("HealthCheckConnectionString");
                        var alertMessage = new NotificationMessage("", "HealthAlert")
                        {
                            AdditionalData = message
                        };
                        await queueHelper.SendMessageAsync("health-alerts", Newtonsoft.Json.JsonConvert.SerializeObject(alertMessage));
                        
                    }

                    await Task.Delay(TimeSpan.FromSeconds(4), cancellationToken);
                }
            }, cancellationToken);


            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    Trace.TraceInformation("Working");
                

            //    await Task.Delay(1000);
            //}
        }
    }
}
