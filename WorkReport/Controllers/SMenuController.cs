using Microsoft.AspNetCore.Mvc;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Models.ViewModel;
using WorkReport.Repositories.Models;

namespace WorkReport.Controllers
{
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
        public IActionResult Test()
        {
            return View();
        }

        /// <summary>
        /// 获取全部的菜单
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSMenuList()
        {
            //var result = _ISMenuService.GetSMenuList(0);
            List<SMenuViewModel> result = null;
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取全部的菜单
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSMenu(BaseQuery baseQuery)
        {
            var rsult = _ISMenuService.GetSMenu(baseQuery);
            return new JsonResult(rsult.Data);
        }

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
        public IActionResult DeleteSMenu(int ID)
        {
            _ISMenuService.Delete<SMenu>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

    }
}
