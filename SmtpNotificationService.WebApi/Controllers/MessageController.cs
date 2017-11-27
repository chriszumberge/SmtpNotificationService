using SmtpNotificationService.WebApi.Models;
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
    /// The Message Controller is to allow users to incrementally build mail messages, interacting with a persisting object using a message key.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class MessageController : ApiController
    {
        internal class MessageToken
        {
            string _messageKey;
            uint _expirationSeconds;

            public string MessageKey => _messageKey;
            public uint ExpirationSeconds => _expirationSeconds;

            public MessageToken(string messageKey, uint expirationSeconds)
            {
                _messageKey = messageKey;
                _expirationSeconds = expirationSeconds;
            }
        }

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
        /// <returns>
        /// Token with message key and number of seconds until the message expires on the server.
        /// </returns>
        [HttpGet]
        [Route("api/Message/New")]
        [ResponseType(typeof(MessageToken))]
        public IHttpActionResult NewMessage()
        {
            string msgKey = _msgSvc.CreateNewMessage();
            // default sender info since it's not required to set
            _msgSvc.SetSender(msgKey, accountEmailAddress, accountDisplayName);

            uint seconds = _msgSvc.GetSecondsUntilExpiration(msgKey);

            MessageToken token = new MessageToken(msgKey, seconds);

            return Ok(token);
        }

        /// <summary>
        /// Sets the recipient parameters of the message.
        /// </summary>
        /// <param name="key">The message's key.</param>
        /// <param name="toAddress">Recipient's mail address.</param>
        /// <param name="toDisplayName">Optional name to display for the recipient.</param>
        /// <returns>Http Action Result indicating if successful or not.</returns>
        [HttpPost]
        [Route("api/Message/To")]
        public IHttpActionResult To(string key, string toAddress, string toDisplayName = "")
        {
            try
            {
                _msgSvc.SetRecipient(key, toAddress, toDisplayName);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Sets the sender parameters of the message.
        /// </summary>
        /// <param name="key">The message's key.</param>
        /// <param name="fromDisplayName">Optional name to display for the sender.</param>
        /// <returns>Http Action Result indicating if successful or not.</returns>
        [HttpPost]
        [Route("api/Message/From")]
        public IHttpActionResult From(string key, string fromDisplayName = "")
        {
            try
            {
                // from address defaults to the smtp sender anyway, just let the user set a display name
                _msgSvc.SetSender(key, fromDisplayName);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Sets the subject parameter of the message.
        /// </summary>
        /// <param name="key">The message's key.</param>
        /// <param name="subject">The subject to set.</param>
        /// <returns>Http Action Result indicating if successful or not.</returns>
        [HttpPost]
        [Route("api/Message/Subject")]
        public IHttpActionResult Subject(string key, string subject)
        {
            try
            {
                _msgSvc.SetSubject(key, subject);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Sets the body parameter of the message.
        /// </summary>
        /// <param name="key">The message's key.</param>
        /// <param name="body">The body to set.</param>
        /// <returns>Http Action Result indicating if successful or not.</returns>
        [HttpPost]
        [Route("api/Message/Body")]
        public IHttpActionResult Body(string key, string body)
        {
            try
            {
                _msgSvc.SetBody(key, body);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Adds an attachment to the message
        /// </summary>
        /// <param name="key">The message's key.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Message/AddAttachment")]
        public IHttpActionResult AddAttachment(string key)
        {
            return InternalServerError(new NotImplementedException());
        }

        /// <summary>
        /// Sends the message with the specified key.
        /// </summary>
        /// <param name="key">The message's key.</param>
        /// <returns>Http Action Result indicating if the send is successful or not.</returns>
        [HttpPost]
        [Route("api/Message/Send")]
        public IHttpActionResult Send(string key)
        {
            try
            {
                _msgSvc.SendMessage(key);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
