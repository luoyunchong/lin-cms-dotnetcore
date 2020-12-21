using System.Threading;
using System.Threading.Tasks;
using LinCms.Dependency;
using MimeKit;

namespace LinCms.Email
{
    public interface IEmailSender: ITransientDependency
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="mail">Mail to be sent</param>
        /// <param name="normalize">
        /// Should normalize email?
        /// If true, it sets sender address/name if it's not set before and makes mail encoding UTF-8.
        /// </param>
        Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default);
    }
}
