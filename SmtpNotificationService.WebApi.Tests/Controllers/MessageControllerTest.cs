using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmtpNotificationService.WebApi.Controllers;
using System.Web.Http;
using System.Web.Http.Results;
using static SmtpNotificationService.WebApi.Controllers.MessageController;

namespace SmtpNotificationService.WebApi.Tests.Controllers
{
    [TestClass]
    public class MessageControllerTest
    {
        [TestMethod]
        public void Post()
        {
            MessageController ctrl = new MessageController();
            
            IHttpActionResult actionResult = ctrl.NewMessage();
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<MessageController.MessageToken>));

            OkNegotiatedContentResult<MessageToken> conNegResult = (OkNegotiatedContentResult<MessageToken>)actionResult;
            MessageToken token = conNegResult.Content;

            ctrl.To(token.MessageKey, "chriszumberge@gmail.com", "Christopher Zumberge");
            ctrl.From(token.MessageKey, "Some Sender Name");
            ctrl.Subject(token.MessageKey, "Test Controller");
            ctrl.Body(token.MessageKey, "Testing the Messages Controller");

            ctrl.Send(token.MessageKey);
        }
    }
}
