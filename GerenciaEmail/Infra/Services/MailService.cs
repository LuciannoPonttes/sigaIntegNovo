using System.Net;
using System.Net.Mail;
using System.Reflection;


namespace GerenciaEmail.Infra.Services
{
    public class MailService : IMailService
    {
        private string SmtpAddress => Environment.GetEnvironmentVariable("SMTP_ADDRESS");
        private int PortNumber => int.Parse(Environment.GetEnvironmentVariable("PORT_NUMBER"));
        private string EmailFromAddress => Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS");
        private string UrlSigaDoc => Environment.GetEnvironmentVariable("URL_DOC_EMAIL_SIGADOC");


        public void AddEmailsToMailMessage(MailMessage mailMessage, string[] emails)
        {
            foreach (var email in emails)
            {
                mailMessage.To.Add(email);
            }
        }

        public void SendMail(string doc, string[] emails, string subject, string body, bool isHtml = false)
        {
            
            using (MailMessage mailMessage = new MailMessage())
            {
                string assunto = "[SIGADoc Notificação] " + subject + " " + doc;

                 
                 // Formata o corpo do e-mail com o link, apenas se isHtml for true
                 string bodyFinal;
                if (isHtml)
                {
                    // Verifica e substitui quebras de linha por <br> se existir
                    string formattedBody = body.Contains("\n") ? body.Replace("\n", "<br />") : body;

                    bodyFinal = $@"
                     <p>O documento <a href='{UrlSigaDoc}{doc}'>{doc}</a> foi assinado.</p>
                     <p>{formattedBody}</p>";
                }
                else
                {
                    // Mantém as quebras de linha normais para texto puro
                    bodyFinal = $"O documento {doc} foi assinado.\n\n{body}\n\nVeja o documento no seguinte link: https://sigadoc.infraero.gov.br/sigaex/app/expediente/doc/exibir?sigla={doc}";
                }

                mailMessage.From = new MailAddress(EmailFromAddress);
                AddEmailsToMailMessage(mailMessage, emails);
                mailMessage.Subject = assunto;
                mailMessage.Body = bodyFinal;
                mailMessage.IsBodyHtml = isHtml;

                using (SmtpClient smtp = new SmtpClient(SmtpAddress, PortNumber))
                {
                    smtp.UseDefaultCredentials = true;
                    smtp.Send(mailMessage);
                }
            }
        }
    }
}
