using IdentityServer4;
using IdentityServer4.Services;
using LinCms.Cms.Users;
using LinCms.Entities;
using LinCms.IRepositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LinCms.IdentityServer4.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IUserIdentityService userIdentityService;
        private readonly IIdentityServerInteractionService _interaction;

        public AccountController(IUserRepository userRepository, IUserIdentityService userIdentityService, IIdentityServerInteractionService interaction)
        {
            this.userRepository = userRepository;
            this.userIdentityService = userIdentityService;
            _interaction = interaction;
        }


        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            var viewModel = new LoginViewModel { Username = "admin", Password = "123qwe", ReturnUrl = returnUrl };

            return View(viewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel viewModel)
        {
            LinUser user = await userRepository.GetUserAsync(r => r.Username == viewModel.Username || r.Email == viewModel.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "用户不存在");
                viewModel.Password = string.Empty;
                return View(viewModel);
            }

            bool valid = await userIdentityService.VerifyUserPasswordAsync(user.Id, viewModel.Password, user.Salt);

            if (!valid)
            {
                ModelState.AddModelError("", "Invalid username or password");
                viewModel.Password = string.Empty;
                return View(viewModel);
            }

            // Use an IdentityServer-compatible ClaimsPrincipal
            var identityServerUser = new IdentityServerUser(user.Id.ToString());
            identityServerUser.DisplayName = viewModel.Username;

            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identityServerUser.CreatePrincipal());
            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(1))
            };
            await HttpContext.SignInAsync(identityServerUser, props);

            return Redirect(viewModel.ReturnUrl);
        }

        [HttpGet("account/logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var context = await _interaction.GetLogoutContextAsync(logoutId);
            return RedirectToAction("Login");
        }

    }

    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
