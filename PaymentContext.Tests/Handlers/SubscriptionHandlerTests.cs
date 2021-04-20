using System;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Handlers;
using PaymentContext.Tests.Mocks;
using PaymentContext.Domain.Commands;

namespace PaymentContext.Tests.Handlers
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ShouldReturnErrorWhenDocumentExists()
        {
            var handler = new SubscriptionHandler(new FakeStudentRepository(), new FakeEmailService());
            var command = new CreateBoletoSubscriptionCommand();
            
            command.FirstName = "Bruce";
            command.LastName = "Wayne";
            command.Document = "999999998";
            command.Email = "kagi.carvalh@outlook.com";

            command.BarCode = "123456789";
            command.BoletoNumber = "12345678910";

            command.PaymentNumber = "123321";
            command.PaidDate = DateTime.Now;
            command.ExpireDate = DateTime.Now.AddDays(2);
            command.Total = 60;
            command.TotalPaid = 60;
            command.Payer = "WAYNE CORP";
            command.PayerDocuments = "12345678911";
            command.PayerDocumentsType = EDocumentType.CPF;
            command.PaymentEmail = "batman@wayne.com";

            command.Street = "teste";
            command.Number = "testa";
            command.Neighborhood = "teste";
            command.City = "testas";
            command.State = "teste";
            command.Country = "testass";
            command.ZipCode = "29123567";

            handler.Handle(command);
            Assert.AreEqual(false, handler.Valid);
        }
    }
}