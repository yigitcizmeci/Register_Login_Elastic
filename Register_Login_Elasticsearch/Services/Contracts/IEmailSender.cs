﻿namespace Register_Login_Elasticsearch.Services.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
