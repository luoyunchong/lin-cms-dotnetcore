using System.Threading;
using System.Threading.Tasks;
using LinCms.Dependency;
using MimeKit;

namespace LinCms.Email
{
    public interface IEmailSender : ITransientDependency
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="message">Mail to be sent</param>
        /// <param name="cancellationToken"></param>
        Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default);
    }
}
