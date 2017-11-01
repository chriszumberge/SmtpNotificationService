using System;
using System.Net;
using System.Net.Mail;
using System.Web.Http;

namespace SmtpNotificationService.WebApi.Controllers
{
    /// <summary>
    /// Mail endpoint.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class MailController : ApiController
    {
        const string accountEmailAddress = "smtpnotificationservice@gmail.com";
        const string accountDisplayName = "Smtp Notification Service";

        string accountPassword => System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];


        /// <summary>
        /// Sends the specified information as an email to the specified mail address.
        /// </summary>
        /// <param name="recipientAddress">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <param name="recipientDisplayName">The optionally included name to show in place of the recipient email address.</param>
        /// <param name="senderDisplayName">The optionally included name to show in place of the sender email address.</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Send(string recipientAddress, string subject, string body, string recipientDisplayName = "", string senderDisplayName = "")
        {
            var fromAddress = new MailAddress(accountEmailAddress, String.IsNullOrEmpty(senderDisplayName) ? accountDisplayName : senderDisplayName);
            var toAddress = new MailAddress(recipientAddress, String.IsNullOrEmpty(recipientDisplayName) ? recipientAddress : recipientDisplayName);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, accountPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

            return Ok();
        }
    }
}
