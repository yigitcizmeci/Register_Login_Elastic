using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Services.Contracts;

namespace Register_Login_Elasticsearch.Security
{
    public class Verification_Code
    {
        private readonly IEmailSender _emailSender;
        private readonly Users _users;
        public Verification_Code(IEmailSender emailSender, Users users)
        {
            _emailSender = emailSender;
            _users = users;
        }
        public async Task<string> CodeGenerator()
        {
            Random random = new Random();
            var VerificationCode = random.Next(1000, 10000).ToString();

            var reciever = _users.Email.ToString();
            var subject = "Verification Code";
            var message = VerificationCode;
            try
            {
                await _emailSender.SendEmailAsync(reciever, subject, message);
            }
            catch (Exception)
            {
                throw new Exception("Error sending verification code");
            }
            return VerificationCode + "\nUse this code while you are loggin in";
        }
    }
}
