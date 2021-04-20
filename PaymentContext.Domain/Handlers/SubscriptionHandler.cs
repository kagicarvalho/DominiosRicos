using Flunt.Notifications;
using Flunt.Validations;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;
using System;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Services;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler :  Notifiable, 
                                        IHandler<CreateBoletoSubscriptionCommand>,
                                        IHandler<CreateCreditCardSubscriptionCommand>,
                                        IHandler<CreatePayPalSubscriptionCommand>
    {

        private readonly IStudentRepository _studentRepository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository studentRepository, IEmailService emailService)
        {
            _studentRepository = studentRepository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            // Fail Fast Validations
            command.Validate();
            if(command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false,"Não foi possível realizar sua assinatura");
            }
            
            // Verificar se Documento já está cadastrado
            if(_studentRepository.DocumentExists(command.Document))
                AddNotification("Document","Este CPF já está em uso");

            // Verificar se Email já está cadastrado
            if(_studentRepository.DocumentExists(command.Email))
                AddNotification("Email","Este Email já está em uso");

            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            // gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.BarCode, command.BoletoNumber, 
                                            command.PaidDate, command.ExpireDate, 
                                            command.Total, command.TotalPaid, 
                                            command.Payer, new Document(command.PayerDocuments, 
                                                                        command.PayerDocumentsType), address, email);
            
            // Relacionamentos
            subscription.AddPayments(payment);
            student.AddSubscription(subscription);

            //Agrupar as Validações
            AddNotifications(name, document, email, address, student, subscription, subscription, payment);

            // Checar Notificações
            if(Invalid)
                return new CommandResult(false, "Não foi possível realizar sua assinatura");
            
            //Salvar as informações
            _studentRepository.CreateSubscription(student);

            // Enviar E-mail de boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao Curso", "Sua assinatura foi assinada");
            // Retornar informações


            return new CommandResult(true, "Assinatura realizada com sucesso");
        }

        public ICommandResult Handle(CreateCreditCardSubscriptionCommand command)
        {
            // Fail Fast Validations
            command.Validate();
            if(command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false,"Não foi possível realizar sua assinatura");
            }
            
            // Verificar se Documento já está cadastrado
            if(_studentRepository.DocumentExists(command.Document))
                AddNotification("Document","Este CPF já está em uso");

            // Verificar se Email já está cadastrado
            if(_studentRepository.DocumentExists(command.Email))
                AddNotification("Email","Este Email já está em uso");

            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            // gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new CreditCardPayment(command.CardHolderName, command.CardNumber, command.LastTransactionNumber,
                                            command.PaidDate, command.ExpireDate, 
                                            command.Total, command.TotalPaid, 
                                            command.Payer, new Document(command.PayerDocuments, 
                                                                        command.PayerDocumentsType), address, email);
            
            // Relacionamentos
            subscription.AddPayments(payment);
            student.AddSubscription(subscription);

            //Agrupar as Validações
            AddNotifications(name, document, email, address, student, subscription, subscription, payment);

            // Checar Notificações
            if(Invalid)
                return new CommandResult(false, "Não foi possível realizar sua assinatura");
            
            //Salvar as informações
            _studentRepository.CreateSubscription(student);

            // Enviar E-mail de boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao Curso", "Sua assinatura foi assinada");
            // Retornar informações

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            // Fail Fast Validations
            command.Validate();
            if(command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false,"Não foi possível realizar sua assinatura");
            }
            
            // Verificar se Documento já está cadastrado
            if(_studentRepository.DocumentExists(command.Document))
                AddNotification("Document","Este CPF já está em uso");

            // Verificar se Email já está cadastrado
            if(_studentRepository.DocumentExists(command.Email))
                AddNotification("Email","Este Email já está em uso");

            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            // gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(command.TransactionCode,
                                            command.PaidDate, command.ExpireDate, 
                                            command.Total, command.TotalPaid, 
                                            command.Payer, new Document(command.PayerDocuments, 
                                                                        command.PayerDocumentsType), address, email);
            
            // Relacionamentos
            subscription.AddPayments(payment);
            student.AddSubscription(subscription);

            //Agrupar as Validações
            AddNotifications(name, document, email, address, student, subscription, subscription, payment);
            
            // Checar Notificações
            if(Invalid)
                return new CommandResult(false, "Não foi possível realizar sua assinatura");
            
            //Salvar as informações
            _studentRepository.CreateSubscription(student);

            // Enviar E-mail de boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao Curso", "Sua assinatura foi assinada");
            // Retornar informações


            return new CommandResult(true, "Assinatura realizada com sucesso");
        }
    }
}