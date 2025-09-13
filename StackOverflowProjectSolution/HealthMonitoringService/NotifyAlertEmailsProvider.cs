using NotificationContracts;
using PostmarkDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class NotifyAlertEmailsProvider:INotifyAlertEmails
    {
        private readonly string _postmarkApiKey = "caf0a99b-5fe6-49ef-a871-08f9a9d122ba";
        private readonly string _fromEmail = "grahovac.pr100.2021@uns.ac.rs";

        public async Task SendEmailsAsync(List<string> emails, string emailBody)
        {
            var client = new PostmarkClient(_postmarkApiKey);

            foreach (var toEmail in emails)
            {
                var message = new PostmarkMessage
                {
                    From = _fromEmail,
                    To = toEmail,
                    Subject = "Notification",
                    TextBody = emailBody,
                    HtmlBody = $"<p>{emailBody}</p>"
                };

                var result = await client.SendMessageAsync(message);

                if (result.Status != PostmarkStatus.Success)
                {
                    // Ako hoćeš možeš logovati ili baciti exception
                    System.Diagnostics.Debug.WriteLine($"Failed to send email to {toEmail}: {result.Message}");
                }
            }
        }
    }
}
