using SmtpNotificationService.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SmtpNotificationService.WebApi.Services
{
    public class MessageService
    {
        Dictionary<string, DateTime> _expirationDict = new Dictionary<string, DateTime>();
        Dictionary<string, MessageDraft> _messageDict = new Dictionary<string, MessageDraft>();

        readonly uint _expirationSeconds;
        readonly SmtpClient _client;

        public MessageService(SmtpClient client, uint expirationSeconds)
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

        public string CreateNewMessage()
        {
            CleanUp();

            string key = Guid.NewGuid().ToString();

            _expirationDict.Add(key, DateTime.Now.AddSeconds(_expirationSeconds));
            _messageDict.Add(key, new MessageDraft(key));

            return key;
        }

        public void SetRecipient(string key, string recipientAddress, string recipientDislayName)
        {
            MessageDraft message = _messageDict[key];
            message.RecipientAddress = recipientAddress;
            message.RecipientDisplayName = recipientDislayName;
        }

        public void AddAttachment()
        {
            throw new NotImplementedException();
        }

        public void CancelMessage(string key)
        {
            _expirationDict.Remove(key);
            _messageDict.Remove(key);
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

            }
        }
    }
}