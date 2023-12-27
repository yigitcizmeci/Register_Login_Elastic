using Microsoft.Extensions.Caching.Memory;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Services.Contracts;

namespace Register_Login_Elasticsearch.Security
{
    public class Verification_Code
    {
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memoryCache;
        public Verification_Code(IEmailSender emailSender, IMemoryCache memoryCache)
        {
            _emailSender = emailSender;
            _memoryCache = memoryCache;
        }
        public async Task<string> CodeGenerator(Users newUser)
        {
            Random random = new Random();
            var VerificationCode = random.Next(1000, 10000).ToString();

            var reciever = newUser.Email;
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
            _memoryCache.Set("VerificationCode", VerificationCode, TimeSpan.FromMinutes(5));
            return VerificationCode;
        }
    }
}
