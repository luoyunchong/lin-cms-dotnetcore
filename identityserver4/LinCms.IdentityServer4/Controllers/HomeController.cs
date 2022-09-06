using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.IdentityServer4.Controllers;

[Route("[controller]/[action]")]
public class HomeController : Controller
{
    private readonly IIdentityServerInteractionService identity;

    public HomeController(IIdentityServerInteractionService identity = null)
    {
        this.identity = identity;
    }

    [HttpGet]
    public ActionResult Index()
    {
        return Redirect("/swagger");
    }
    [HttpGet]
    public async Task<IActionResult> Error(string errorId)
    {
        var vm = new ErrorViewModel();

        // retrieve error details from identityserver
        var message = await identity.GetErrorContextAsync(errorId);
        if (message != null)
        {
            vm.Error = message;
        }
        ViewBag.VM = vm;
        return View(vm);
    }
}


public class ErrorViewModel
{
    public ErrorMessage Error { get; set; }
}