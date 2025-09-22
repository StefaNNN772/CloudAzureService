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
using NotificationContracts;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly AzureQueueService queueService = new AzureQueueService();

        private JobServer jobServer = new JobServer();
        private HealthMonitoring hmServer = new HealthMonitoring();

        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            try
            {
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

            Thread.Sleep(60000); 

            bool result = base.OnStart();
            jobServer.Open();
            hmServer.Open();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();
            jobServer.Close();
            hmServer.Close();

            Trace.TraceInformation("NotificationService has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var emailService = new JobServerProvider();
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Check for messages in the queue
                    var message = await queueService.ReceiveMessageAsync();
                    
                    if (message != null)
                    {
                        Trace.TraceInformation($"Processing notification message: {message.MessageType}");
                        
                        // Build email content based on message type
                        string emailBody = "";
                        string subject = "Notification";
                        
                        switch (message.MessageType)
                        {
                            case MessageTypes.BestAnswerSelected:
                                emailBody = CreateBestAnswerEmailBody(message);
                                subject = "Your answer was selected as the best answer!";
                                break;
                                
                            case MessageTypes.ServiceHealthAlert:
                                emailBody = CreateServiceAlertEmailBody(message);
                                subject = "Service Health Alert";
                                break;
                                
                            default:
                                emailBody = "You have a new notification.";
                                break;
                        }
                        
                        // Send emails to all recipients
                        if (message.EmailAddresses != null && message.EmailAddresses.Count > 0)
                        {
                            await emailService.SendEmailsAsync(message.EmailAddresses, emailBody);
                            Trace.TraceInformation($"Sent {message.MessageType} notification to {message.EmailAddresses.Count} recipients");
                        }
                    }
                    else
                    {
                        // No messages in queue, wait a bit before checking again
                        Trace.TraceInformation("No messages in queue, waiting...");
                        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error processing queue message: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
            }
        }
        
        private string CreateBestAnswerEmailBody(NotificationMessage message)
        {
            var questionTitle = message.Data.ContainsKey("QuestionTitle") ? message.Data["QuestionTitle"].ToString() : "Unknown Question";
            var answerAuthorName = message.Data.ContainsKey("AnswerAuthorName") ? message.Data["AnswerAuthorName"].ToString() : "Unknown Author";
            var questionAuthorName = message.Data.ContainsKey("QuestionAuthorName") ? message.Data["QuestionAuthorName"].ToString() : "Unknown Author";
            
            // Generic notification that works for both question author and answer author
            return $@"
                <h2>Great News!</h2>
                <p>A best answer has been selected for the question '<strong>{questionTitle}</strong>'!</p>
                <p><strong>Answer provided by:</strong> {answerAuthorName}</p>
                <p><strong>Question asked by:</strong> {questionAuthorName}</p>
                <p>Thank you for your participation in our community.</p>
                <p>Best regards,<br/>StackOverflow Service Team</p>
            ";
        }
        
        private string CreateServiceAlertEmailBody(NotificationMessage message)
        {
            var serviceName = message.Data.ContainsKey("ServiceName") ? message.Data["ServiceName"].ToString() : "Unknown Service";
            var status = message.Data.ContainsKey("Status") ? message.Data["Status"].ToString() : "Unknown Status";
            var errorMessage = message.Data.ContainsKey("ErrorMessage") ? message.Data["ErrorMessage"].ToString() : "No details available";
            var timestamp = message.Data.ContainsKey("Timestamp") ? message.Data["Timestamp"].ToString() : DateTime.Now.ToString();
            
            return $@"
                <h2>Service Health Alert</h2>
                <p><strong>Service:</strong> {serviceName}</p>
                <p><strong>Status:</strong> {status}</p>
                <p><strong>Time:</strong> {timestamp}</p>
                <p><strong>Details:</strong> {errorMessage}</p>
                <p>Please investigate this issue immediately.</p>
                <p>Monitoring Team</p>
            ";
        }
    }
}
