using Register_Login_Elasticsearch.Services.Contracts;
using System.Net.Mail;
using System.Net;

namespace Register_Login_Elasticsearch.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "test-sender13@outlook.com";
            var pw = "Test.netapi";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };
            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,   
                                subject,
                                message));
        }
    }
}
