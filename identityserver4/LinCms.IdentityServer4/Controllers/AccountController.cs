using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityServer4;
using LinCms.Cms.Users;
using LinCms.IRepositories;
using LinCms.Entities;

namespace LinCms.IdentityServer4.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IUserIdentityService userIdentityService;

        public AccountController(IUserRepository userRepository, IUserIdentityService userIdentityService)
        {
            this.userRepository = userRepository;
            this.userIdentityService = userIdentityService;
        }


        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            var viewModel = new LoginViewModel { Username = "admin", Password = "123qwe", ReturnUrl = returnUrl };

            return View("/Views/Login.cshtml", viewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel viewModel)
        {
            LinUser user = await userRepository.GetUserAsync(r => r.Username == viewModel.Username || r.Email == viewModel.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "用户不存在");
                viewModel.Password = string.Empty;
                return View("/Views/Login.cshtml", viewModel);
            }

            bool valid = await userIdentityService.VerifyUserPasswordAsync(user.Id, viewModel.Password);

            if (!valid)
            {
                ModelState.AddModelError("", "Invalid username or password");
                viewModel.Password = string.Empty;
                return View("/Views/Login.cshtml", viewModel);
            }

            // Use an IdentityServer-compatible ClaimsPrincipal
            var identityServerUser = new IdentityServerUser(viewModel.Username);
            identityServerUser.DisplayName = viewModel.Username;
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identityServerUser.CreatePrincipal());

            return Redirect(viewModel.ReturnUrl);
        }
    }

    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
