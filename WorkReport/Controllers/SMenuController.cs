using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Reflection;
using WorkReport.Commons.Api;
using WorkReport.Commons.Attributes;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;

namespace WorkReport.Controllers
{

    [CustomAuthenticationRemark(ActionRemark = "菜单设置")]
    public class SMenuController : Controller
    {

        private readonly ISMenuService _ISMenuService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public SMenuController(ISMenuService ISMenuService)
        {
            _ISMenuService = ISMenuService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取全部的菜单
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthenticationRemark(ActionRemark = "获取菜单列表List集合", IsShow = false)]
        public IActionResult GetSMenuList()
        {
            var result = _ISMenuService.GetSMenuList();
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取全部的菜单
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthenticationRemark(ActionRemark = "获取菜单列表HttpResponseResult", IsShow = false)]
        public IActionResult GetSMenu(BaseQuery baseQuery)
        {
            var rsult = _ISMenuService.GetSMenu(baseQuery);
            return new JsonResult(rsult.Data);
        }

        [CustomAuthenticationRemark(ActionRemark = "根据ID查询菜单", IsShow = false)]
        /// <summary>
        /// 编辑信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSMenuByID(int ID)
        {
            SMenu sMenu = _ISMenuService.Find<SMenu>(ID);
            return new JsonResult(sMenu);
        }

        /// <summary>
        /// 添加修改方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthenticationRemark(ActionRemark = "菜单添加修改方法")]
        public IActionResult SaveSMenu([FromBody] SMenu sMenu)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (sMenu != null && sMenu.ID > 0)
                {
                    _ISMenuService.Update(sMenu);
                }
                else
                {
                    _ISMenuService.Insert(sMenu);
                }
                doResult = HttpResponseCode.Success;
            }
            catch (Exception ex)
            {
                doResult = HttpResponseCode.Failed;
            }

            return Json(new HttpResponseResult()
            {
                Msg = "保存成功",
                Code = doResult
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthenticationRemark(ActionRemark = "删除菜单")]
        public IActionResult DeleteSMenu(int ID)
        {
            _ISMenuService.Delete<SMenu>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }


        /// <summary>
        /// 更新菜单，根据CustomAuthenticationRemarkAttribute
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCustomAuthenticationRemarkAttribute()
        {
            try
            {
                var assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
                    .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

                assembly.ForEach(r =>
                {
                    foreach (var methodInfo in r.GetMethods())
                    {
                        foreach (Attribute attribute in methodInfo.GetCustomAttributes())
                        {
                            if (attribute is CustomAuthenticationRemarkAttribute authenticationRemark)
                            {
                                authenticationRemark.ControllerName = r.Name;
                                authenticationRemark.ActionName = methodInfo.Name;
                                Console.WriteLine(JsonConvert.SerializeObject(authenticationRemark));
                            }
                        }
                    }
                });

                return Content(
                    JsonConvert.SerializeObject(new HttpResponseResult()
                    {
                        Msg = "菜单更新完成",
                        Code = HttpResponseCode.Success
                    })
                    , "application/json");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
