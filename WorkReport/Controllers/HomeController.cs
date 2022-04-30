using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WorkReport.Commons.Extensions;
using WorkReport.Interface.IService;
using WorkReport.Models;
using WorkReport.Models.ViewModel;

namespace WorkReport.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISMenuService _ISMenuService;

        public HomeController(ILogger<HomeController> logger, ISMenuService ISMenuService)
        {
            _logger = logger;
            _ISMenuService = ISMenuService;
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
        /// 获取系统的首页数据菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetHomeList()
        {
            MenusInfoResultDTO menusInfoResultDTO = new MenusInfoResultDTO();

            int userID = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid).Value.ToInt();
            var result = _ISMenuService.GetSMenuList(userID);
            menusInfoResultDTO.menuInfo = result;
            menusInfoResultDTO.homeInfo = new S_HomeViewModel();
            menusInfoResultDTO.logoInfo = new S_LogoViewModel();

            return new JsonResult(menusInfoResultDTO);
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