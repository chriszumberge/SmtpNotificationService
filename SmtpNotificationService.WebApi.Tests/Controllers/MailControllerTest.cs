using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmtpNotificationService.WebApi.Controllers;

namespace SmtpNotificationService.WebApi.Tests.Controllers
{
    [TestClass]
    public class MailControllerTest
    {
        [TestMethod]
        public void Post()
        {
            MailController controller = new MailController();

            var recipientAddress = "chriszumberge@gmail.com";
            var subject = "Test Controller";
            var body = "Testing the Mail Controller";
            var recipientDisplayName = "Christopher Zumberge";
            var senderDisplayName = "Smtp Notification Service";

            controller.Send(recipientAddress, subject, body, recipientDisplayName, senderDisplayName);
        }
    }
}
