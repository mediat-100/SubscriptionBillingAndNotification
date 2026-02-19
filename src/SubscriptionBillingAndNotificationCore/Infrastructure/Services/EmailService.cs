using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Utilities.Settings;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _emailSettings;

        SmtpClient _smtpClient;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings;
            _smtpClient = new SmtpClient(_emailSettings.Value.Host, _emailSettings.Value.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Value.Username, _emailSettings.Value.Password),
                EnableSsl = true
            };
            
        }
        public async Task SendEmail(string recipient, string subject, string body)
        {
            try
            {
                string from = _emailSettings.Value.From;
                _smtpClient.Send(from, recipient, subject, body);
            }
            catch (Exception ex)
            {
                throw new SendEmailException("An error occured while trying to send email");
            }
        }
    }
}
