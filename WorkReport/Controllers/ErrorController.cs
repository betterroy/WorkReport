using Microsoft.AspNetCore.Mvc;

namespace WorkReport.Controllers
{
    public class ErrorController : Controller
    {
        //[Route("Error/404")]
        //public IActionResult Error404()
        //{
        //    return View();
        //}

        [Route("Error/{code:int}")]
        public IActionResult Error(int code)
        {
            ViewBag.Code = code;
            return View();
        }

    }
}
