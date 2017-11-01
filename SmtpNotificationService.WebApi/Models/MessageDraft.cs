using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SmtpNotificationService.WebApi.Models
{
    public class MessageDraft
    {
        readonly string _id;
        public string Id => _id;

        public string RecipientAddress { get; set; }
        public string RecipientDisplayName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderDisplayName { get; set; }

        public MessageDraft(string id)
        {
            _id = id;
        }

        internal MailMessage ToMailMessage()
        {
            throw new NotImplementedException();
        }
    }
}