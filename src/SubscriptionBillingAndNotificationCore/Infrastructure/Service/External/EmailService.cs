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

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service.External
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
        public void SendEmail(string recipient, string subject, string body)
        {
            string from = _emailSettings.Value.From;
            _smtpClient.Send(from, recipient, subject, body);
        }
    }
}
