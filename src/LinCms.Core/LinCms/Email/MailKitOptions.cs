namespace LinCms.Email
{
    public class MailKitOptions
    {

        /// <summary>
        /// SMTP Host Server address
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// SMTP Host Server Port ,default is 25
        /// </summary>
        public int Port { get; set; } = 25;

        public bool EnableSsl { get; set; } = false;

        /// <summary>
        /// send user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// send user password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Domain name to login to SMTP server.
        /// </summary>
        public string Domain { get; set; }


    }
}
