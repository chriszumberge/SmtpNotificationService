using SmtpNotificationService.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SmtpNotificationService.WebApi.Services
{
    internal class MessageService
    {
        Dictionary<string, DateTime> _expirationDict = new Dictionary<string, DateTime>();
        Dictionary<string, MessageDraft> _messageDict = new Dictionary<string, MessageDraft>();

        readonly uint _expirationSeconds;
        readonly SmtpClient _client;

        internal MessageService(SmtpClient client, uint expirationSeconds)
        {
            _client = client;
            _expirationSeconds = expirationSeconds;
        }

        private void CleanUp()
        {
            List<string> keysToRemove = new List<string>();
            foreach (var pair in _expirationDict)
            {
                if (DateTime.Now > pair.Value)
                {
                    keysToRemove.Add(pair.Key);
                }
            }

            foreach (string key in keysToRemove)
            {
                this.CancelMessage(key);
            }
        }

        internal string CreateNewMessage()
        {
            CleanUp();

            string key = Guid.NewGuid().ToString();

            _expirationDict.Add(key, DateTime.Now.AddSeconds(_expirationSeconds));
            _messageDict.Add(key, new MessageDraft(key));

            return key;
        }

        internal void SetRecipient(string key, string recipientAddress, string recipientDislayName)
        {
            MessageDraft message = _messageDict[key];
            message.RecipientAddress = recipientAddress;
            message.RecipientDisplayName = recipientDislayName;
        }

        internal void AddAttachment()
        {
            throw new NotImplementedException();
        }

        internal void CancelMessage(string key)
        {
            _expirationDict.Remove(key);
            _messageDict.Remove(key);
        }

        internal void KeepAlive(string key, uint additionalExpirationSeconds) => _expirationDict[key].AddSeconds(additionalExpirationSeconds);

        internal DateTime GetExpiration(string key) => _expirationDict[key];

        internal uint GetSecondsUntilExpiration(string key) => (uint)(Math.Round((DateTime.Now - _expirationDict[key]).TotalSeconds));

        internal void SetSender(string key, string fromAddress, string fromDisplayName)
        {
            MessageDraft message = _messageDict[key];
            message.SenderAddress = fromAddress;
            message.SenderDisplayName = fromDisplayName;
        }

        internal void SetSender(string key, string fromDisplayName)
        {
            MessageDraft message = _messageDict[key];
            message.SenderDisplayName = fromDisplayName;
        }

        internal void SetSubject(string key, string subject)
        {
            MessageDraft message = _messageDict[key];
            message.Subject = subject;
        }

        internal void SetBody(string key, string body)
        {
            MessageDraft message = _messageDict[key];
            message.Body = body;
        }

        public async void SendMessage(string key)
        {
            var message = _messageDict[key];

            try
            {
                using (MailMessage mailMessage = message.ToMailMessage())
                {
                    _client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}