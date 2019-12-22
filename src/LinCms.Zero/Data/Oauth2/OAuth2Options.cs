namespace LinCms.Zero.Data.Oauth2
{
    public class OAuth2Options
    {
        public class GitHub
        {
            public string ApplicationName { get; set; }
            public string ClientID { get; set; }
            public string ClientSecret { get; set; }
            public string CallBackUrl { get; set; }

            public string RequestUrl { get; set; }

        }
    }
}
