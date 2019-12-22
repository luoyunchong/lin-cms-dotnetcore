using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using IdentityServer4.Services;
using LinCms.Zero.Data.Oauth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/oauth2")]
    [ApiController]
    public class Oauth2Controller : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly OAuth2Options _auth2Options;

        private readonly IIdentityServerInteractionService _idsInteraction;
        public Oauth2Controller(IHttpClientFactory clientFactory, IOptionsMonitor<OAuth2Options> oauthOptionsMonitor)
        {
            _clientFactory = clientFactory;
            _auth2Options = oauthOptionsMonitor.CurrentValue;
        }

        [HttpGet("github")]
        public IActionResult Oauth2Github(string returnUrl)
        {
            return null;

        }
    }

}