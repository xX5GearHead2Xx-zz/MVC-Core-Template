using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;

namespace Ecommerce.Azure
{
    public class Communication
    {
        public static bool SendEmail(string Subject, string Message, List<string> Recipients)
        {
            try
            {
                string connectionString = Configuration.configuration["EmailSettings:ConnectionString"];
                EmailClient EmailClient = new EmailClient(connectionString);
                EmailContent emailContent = new EmailContent(Subject);
                emailContent.Html = Message;
                List<EmailAddress> emailAddresses = new List<EmailAddress>();
                foreach (string Address in Recipients)
                {
                    emailAddresses.Add(new EmailAddress(Address) { DisplayName = Address });
                }
                EmailRecipients emailRecipients = new EmailRecipients(emailAddresses);
                EmailMessage emailMessage = new EmailMessage(Configuration.configuration["EmailSettings:SiteEmail"], emailContent, emailRecipients);
                SendEmailResult emailResult = EmailClient.Send(emailMessage, CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
