using Microsoft.AspNetCore.Mvc;

namespace WorkReport.AuthorizationCenter.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
