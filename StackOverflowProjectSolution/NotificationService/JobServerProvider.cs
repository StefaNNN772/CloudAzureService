using NotificationContracts;
using PostmarkDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class JobServerProvider : INotification
    {
        //private readonly string _postmarkApiKey = "481459ee-0b66-45f6-8a87-47a7ad46da2b"; 
        //private readonly string _fromEmail = "grahovac.pr100.2021@uns.ac.rs";    

        //public async Task SendEmailsAsync(List<string> emails, string emailBody)
        //{
        //    var client = new PostmarkClient(_postmarkApiKey);

        //    foreach (var toEmail in emails)
        //    {
        //        var message = new PostmarkMessage
        //        {
        //            From = _fromEmail,
        //            To = toEmail,
        //            Subject = "Notification",
        //            TextBody = emailBody,
        //            HtmlBody = $"<p>{emailBody}</p>"
        //        };

        //        var result = await client.SendMessageAsync(message);

        //        if (result.Status != PostmarkStatus.Success)
        //        {
        //            // Ako hoćeš možeš logovati ili baciti exception
        //            System.Diagnostics.Debug.WriteLine($"Failed to send email to {toEmail}: {result.Message}");
        //        }
        //    }
        //}

        private string smtpServer = "smtp.gmail.com";
        private int port = 587;
        private string senderEmail = "schneiderelectricinternship@gmail.com";
        private string senderPassword = "aeoo vuqm gifg pnsw";

        public async Task SendEmailsAsync(List<string> emails, string emailBody)
        {
            using (var smtp = new SmtpClient(smtpServer, port))
            {
                smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtp.EnableSsl = true;

                foreach (var toEmail in emails)
                {
                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(senderEmail);
                        mail.To.Add(toEmail);
                        mail.Subject = "Notification";
                        mail.Body = emailBody;
                        mail.IsBodyHtml = true;

                        try
                        {
                            await smtp.SendMailAsync(mail);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to send email to {toEmail}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
