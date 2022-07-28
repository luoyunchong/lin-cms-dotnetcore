using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Email;
using LinCms.Data.Options;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LinCms.Cms.Account
{
    public class EmailData
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Link { get; set; }

        public EmailData(IEnumerable<string> to, string subject, string link)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(address => new MailboxAddress(address, address)));

            Subject = subject;
            Link = link;
        }
    }
    public class EmailFactory : IEmalFactory
    {
        private readonly IEmailSender _emailSender;
        private readonly MailKitOptions _mailKitOptions;
        public EmailFactory(IEmailSender emailSender, IOptionsMonitor<MailKitOptions> options)
        {
            _emailSender = emailSender;
            _mailKitOptions = options.CurrentValue;
        }
        private MimeMessage Create(EmailData data)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
            mimeMessage.To.AddRange(data.To);
            mimeMessage.Subject = data.Subject;

            return mimeMessage;
        }

        public Task SendConfirmationEmailAsync(EmailData data)
        {
            throw new NotImplementedException();
        }
    }

    public interface IEmalFactory
    {
        Task SendConfirmationEmailAsync(EmailData data);
    }
}
