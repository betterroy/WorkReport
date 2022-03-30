using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WorkReport.Models;

namespace WorkReport.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 系统主页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 返回添加Report页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Redirect(string path)
        {
            //HttpContext.Response.Redirect("UReport/GetCurrentComment");
            return View(path);
        }

    }
}