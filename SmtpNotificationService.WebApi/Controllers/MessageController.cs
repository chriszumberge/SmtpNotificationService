using SmtpNotificationService.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Web.Http.Description;

namespace SmtpNotificationService.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class MessageController : ApiController
    {
        const string accountEmailAddress = "smtpnotificationservice@gmail.com";
        const string accountDisplayName = "Smtp Notification Service";

        MessageService _msgSvc = new MessageService(new SmtpClient()
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(accountEmailAddress, System.Configuration.ConfigurationManager.AppSettings["EmailPassword"])
        }, (uint)TimeSpan.FromHours(5).TotalSeconds);

        /// <summary>
        /// Creates a new message with the message service and returns the id required for interacting with it.
        /// </summary>
        /// <returns>Message id</returns>
        [HttpGet]
        [Route("api/Message/New")]
        [ResponseType(typeof(string))]
        public IHttpActionResult NewMessage()
        {
            string msgKey = _msgSvc.CreateNewMessage();
            return Ok(msgKey);
        }

        [HttpPost]
        [Route("api/Message/To")]
        public IHttpActionResult To(string key, string toAddress, string toDisplayName = "")
        {

        }
    }
}
