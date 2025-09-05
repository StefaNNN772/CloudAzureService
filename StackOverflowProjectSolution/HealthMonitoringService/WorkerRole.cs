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

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly HealthCheckRepository _healthCheckRepository = new HealthCheckRepository();

        private JobServer jobServer = new JobServer();
        private AdminAlertEmailsServer aaeServer = new AdminAlertEmailsServer();
        
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

            jobServer.Open();
            aaeServer.Open();

            Trace.TraceInformation("HealthMonitoringService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            jobServer.Close();
            aaeServer.Close();

            Trace.TraceInformation("HealthMonitoringService has stopped");
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

            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        bool isAliveSO = healthMonitoringStackOverflowService.CheckServices();
                        Trace.TraceInformation(isAliveSO ? "StackOverflowService ok" : "StackOverflowService not_ok");

                        string rowKey = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                        HealthCheck healthCheckSO = new HealthCheck(rowKey);

                        healthCheckSO.Date = DateTime.UtcNow;
                        healthCheckSO.ServiceName = "StackOverflowService";
                        healthCheckSO.Status = isAliveSO ? "ok" : "not_ok";

                        _healthCheckRepository.AddHealthCheck(healthCheckSO);

                        bool isAliveNotification = healthMonitoringNotificationService.CheckServices();
                        Trace.TraceInformation(isAliveNotification ? "NotificationService ok" : "NotificationService not_ok");

                        rowKey = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                        HealthCheck healthCheckNO = new HealthCheck(rowKey);

                        healthCheckNO.Date = DateTime.UtcNow;
                        healthCheckNO.ServiceName = "NotificationService";
                        healthCheckNO.Status = isAliveNotification ? "ok" : "not_ok";

                        _healthCheckRepository.AddHealthCheck(healthCheckNO);

                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError($"not_ok - {ex.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(4), cancellationToken);
                }
            }, cancellationToken);


            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                

                await Task.Delay(1000);
            }
        }
    }
}
