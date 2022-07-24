using System.Threading.Tasks;
using IGeekFan.FreeKit.Email;
using MimeKit;
using Xunit;
using Xunit.Abstractions;


namespace LinCms.Test.Core
{
    public class EmailSenderTest : BaseLinCmsTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly IEmailSender _emailSender;
        public EmailSenderTest(ITestOutputHelper testOut)
        {
            testOutputHelper = testOut;
            _emailSender = GetService<IEmailSender>();
        }

        [Fact]
        public async Task OutputTest()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("igeekfan", "igeekfan@163.com"));
            message.To.Add(new MailboxAddress("Mrs. luoyunchong", "luoyunchong@foxmail.com"));
            message.Subject = "How you doin'?";

            message.Body = new TextPart("plain")
            {
                Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"

            };
            await _emailSender.SendAsync(message);

            await Task.CompletedTask;
        }
    }
}
