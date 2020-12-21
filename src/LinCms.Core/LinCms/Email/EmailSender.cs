using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
namespace LinCms.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly MailKitOptions options;

        public EmailSender(IOptions<MailKitOptions> options)
        {
            this.options = options.Value;
        }

        public async Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
        {

            using (SmtpClient client = new SmtpClient())
            {

#pragma warning disable CS0618 // Type or member is obsolete
                client.SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls;
#pragma warning restore CS0618 // Type or member is obsolete
                client.Connect(options.Host, options.Port, options.EnableSsl == true ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);


                NetworkCredential credential = !string.IsNullOrEmpty(options.Domain)
                           ? new NetworkCredential(options.UserName, options.Password, options.Domain)
                           : new NetworkCredential(options.UserName, options.Password);

                client.Authenticate(credential, cancellationToken);

                await client.SendAsync(message, cancellationToken);
                client.Disconnect(true);
            }
        }


    }
}
