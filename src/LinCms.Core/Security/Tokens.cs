using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Core.Security
{
    public class Tokens
    {
        public Tokens(string accessToken, string refreshToken)
        {
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
        }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public override string ToString()
        {
            return $"Tokns AccessToken:{AccessToken},RefreshToken:{RefreshToken}";
        }
    }
}
