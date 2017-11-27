using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SmtpNotificationService.WebApi.Models
{
    internal class MessageDraft
    {
        readonly string _id;
        public string Id => _id;

        public string RecipientAddress { get; set; }
        public string RecipientDisplayName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderDisplayName { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public MessageDraft(string id)
        {
            _id = id;
        }

        internal MailMessage ToMailMessage()
        {
            if (String.IsNullOrEmpty(RecipientAddress))
                throw new Exception($"Property '{nameof(RecipientAddress)}' cannot be null or empty.");
            if (String.IsNullOrEmpty(SenderAddress))
                throw new Exception($"Property '{nameof(SenderAddress)}' cannot be null or empty.");

            MailAddress senderMailAddress;
            if (String.IsNullOrEmpty(SenderDisplayName))
                senderMailAddress = new MailAddress(SenderAddress);
            else
                senderMailAddress = new MailAddress(SenderAddress, SenderDisplayName);

            MailAddress recipientMailAddress;
            if (String.IsNullOrEmpty(RecipientDisplayName))
                recipientMailAddress = new MailAddress(RecipientAddress);
            else
                recipientMailAddress = new MailAddress(RecipientAddress, RecipientDisplayName);

            MailMessage message = new MailMessage(senderMailAddress, recipientMailAddress);

            if (!String.IsNullOrEmpty(Subject))
                message.Subject = Subject;

            if (!String.IsNullOrEmpty(Body))
                message.Body = Body;

            return message;
        }
    }
}