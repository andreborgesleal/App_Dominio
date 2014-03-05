using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SendGridMail;
using App_Dominio.Contratos;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;


namespace App_Dominio.Component
{
    public class SendEmail
    {
        private SendGrid myMessage { get; set; }

        private void Prepare(MailAddress sender, List<String> recipients, string Html, string Subject, string Text)
        {
            myMessage = SendGrid.GetInstance();

            // Add the message properties.
            myMessage.From = sender;

            // Add multiple addresses to the To field.
            //recipients = new List<String>()
            //{
            //    "Jeff Smith <jeff@contoso.com>",
            //    "Anna Lidman <anna@contoso.com>",
            //    "Peter Saddow <peter@contoso.com>"
            //};

            foreach (string recipient in recipients)
            {
                myMessage.AddTo(recipient);
            }
            myMessage.AddBcc("André Borges Leal <andreborgesleal@live.com>");
            myMessage.Html = Html;
            myMessage.Subject = Subject;
            myMessage.Text = Text;
        }


        public Validate Send(MailAddress sender, List<String> recipients, string Html, string Subject, string Text)
        {
            Validate result = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            Prepare(sender, recipients, Html, Subject, Text);

            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["smtp_account"], System.Configuration.ConfigurationManager.AppSettings["smtp_pwd"]);
            //credentials.Domain = System.Configuration.ConfigurationManager.AppSettings["email_host"];

            // Create an SMTP transport for sending email.
            var transportWeb = Web.GetInstance(credentials);

            // Send the email.
            try
            {
                transportWeb.Deliver(myMessage);
            }
            catch(Exception ex)
            {
                result.Code = 15;
                result.Message = MensagemPadrao.Message(15).ToString();
                result.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                result.MessageType = MsgType.ERROR;
            }

            return result;
        }
    }
}
