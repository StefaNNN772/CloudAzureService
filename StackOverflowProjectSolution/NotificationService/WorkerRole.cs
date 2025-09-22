using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using DatabaseRepository.Helpers;
using DatabaseRepository.Models;
using DatabaseRepository.Repositories;
using NotificationContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

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
            var queueHelper = new QueueHelper("DataConnectionString");
            var answerRepo = new AnswerRepository();
            var questionRepo = new QuestionRepository();
            var userRepo = new UserRepository();
            var alertEmailsRepo = new AlertEmailsRepository();
            var notificationsLogRepo = new NotificationsLogRepository();
            var notificationService = new JobServerProvider();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Check for messages in the notifications queue
                    var notificationMessage = await queueHelper.ReceiveMessageAsync("notifications", TimeSpan.FromMinutes(5));
                    var healthAlertMessage = await queueHelper.ReceiveMessageAsync("health-alerts", TimeSpan.FromMinutes(5));

                    if (notificationMessage != null)
                    {
                        try
                        {
                            // Deserialize the message
                            var notification = Newtonsoft.Json.JsonConvert.DeserializeObject<NotificationMessage>(notificationMessage.AsString);

                            if (notification.MessageType == "BestAnswerSelected")
                            {
                                await ProcessBestAnswerNotification(notification.AnswerId, answerRepo, questionRepo, userRepo, notificationService, notificationsLogRepo);
                            }

                            // Delete the processed message from the queue
                            await queueHelper.DeleteMessageAsync("notifications", notificationMessage);

                            Trace.TraceInformation($"Processed notification message: {notification.MessageType} for AnswerId: {notification.AnswerId}");
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError($"Error processing notification message: {ex.Message}");
                            // Message will be returned to queue after visibility timeout
                        }
                    }

                    if (healthAlertMessage != null)
                    {
                        try
                        {
                            // Deserialize the health alert message
                            var healthAlert = Newtonsoft.Json.JsonConvert.DeserializeObject<NotificationMessage>(healthAlertMessage.AsString);

                            if (healthAlert.MessageType == "HealthAlert")
                            {
                                await ProcessHealthAlert(healthAlert.AdditionalData, alertEmailsRepo, notificationService);
                            }

                            // Delete the processed message from the queue
                            await queueHelper.DeleteMessageAsync("health-alerts", healthAlertMessage);

                            Trace.TraceInformation($"Processed health alert message: {healthAlert.AdditionalData}");
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError($"Error processing health alert message: {ex.Message}");
                            // Message will be returned to queue after visibility timeout
                        }
                    }

                    if (notificationMessage == null && healthAlertMessage == null)
                    {
                        // No messages available, wait a bit before checking again
                        await Task.Delay(5000, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error in notification processing loop: {ex.Message}");
                    await Task.Delay(10000, cancellationToken);
                }
            }
        }

        private async Task ProcessHealthAlert(string alertMessage, AlertEmailsRepository alertEmailsRepo, INotification notificationService)
        {
            try
            {
                // Get all alert email addresses
                var alertEmails = alertEmailsRepo.GetAllAlertEmails().ToList();
                var emailAddresses = alertEmails.Select(e => e.Email).ToList();

                if (emailAddresses.Any())
                {
                    string emailBody = $@"
                        <h2>Health Monitoring Alert</h2>
                        <p><strong>Alert:</strong> Service health check failed</p>
                        <p><strong>Details:</strong> {alertMessage}</p>
                        <p><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                        <p>Please check the service status and take appropriate action.</p>
                    ";

                    // Send alert emails
                    await notificationService.SendEmailsAsync(emailAddresses, emailBody);

                    Trace.TraceInformation($"Sent health alert emails to {emailAddresses.Count} recipients");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error processing health alert: {ex.Message}");
                throw;
            }
        }

        private async Task ProcessBestAnswerNotification(string answerId, AnswerRepository answerRepo, QuestionRepository questionRepo, UserRepository userRepo, INotification notificationService, NotificationsLogRepository notificationsLogRepo)
        {
            try
            {
                // Find the answer
                var answer = answerRepo.RetrieveAllAnswers().FirstOrDefault(a => a.RowKey == answerId);
                if (answer == null)
                {
                    Trace.TraceWarning($"Answer with ID {answerId} not found");
                    return;
                }

                // Find the question
                var question = questionRepo.RetrieveAllQuestions().FirstOrDefault(q => q.RowKey == answer.QuestionId);
                if (question == null)
                {
                    Trace.TraceWarning($"Question with ID {answer.QuestionId} not found");
                    return;
                }

                // Find all users who answered this question
                var allAnswers = answerRepo.RetrieveAllAnswers().Where(a => a.QuestionId == answer.QuestionId).ToList();
                var userIds = allAnswers.Select(a => a.UserId).Distinct().ToList();

                // Get email addresses
                var emails = new List<string>();
                foreach (var userId in userIds)
                {
                    var user = userRepo.RetrieveAllUsers().FirstOrDefault(u => u.RowKey == userId);
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        emails.Add(user.Email);
                    }
                }

                if (emails.Any())
                {
                    // Get the best answer author
                    var bestAnswerAuthor = userRepo.RetrieveAllUsers().FirstOrDefault(u => u.RowKey == answer.UserId);
                    string authorName = bestAnswerAuthor != null ? $"{bestAnswerAuthor.FirstName} {bestAnswerAuthor.LastName}" : "Unknown";

                    // Create email body
                    string emailBody = $@"
                        <h2>Question Closed - Best Answer Selected</h2>
                        <p><strong>Question:</strong> {question.Title}</p>
                        <p><strong>Best Answer Author:</strong> {authorName}</p>
                        <p><strong>Best Answer:</strong></p>
                        <div>{answer.Body}</div>
                        <p>The question has been successfully closed with the above answer marked as the best solution.</p>
                    ";

                    // Send emails
                    await notificationService.SendEmailsAsync(emails, emailBody);

                    // Log the notification
                    var notificationLog = new NotificationsLog(Guid.NewGuid().ToString())
                    {
                        AnswerId = answerId,
                        EmailsSent = emails.Count,
                        SentAt = DateTime.UtcNow
                    };
                    notificationsLogRepo.AddNotificationsLog(notificationLog);

                    Trace.TraceInformation($"Sent notification emails for answer {answerId} to {emails.Count} recipients");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error processing best answer notification for {answerId}: {ex.Message}");
                throw;
            }
        }
    }
}
