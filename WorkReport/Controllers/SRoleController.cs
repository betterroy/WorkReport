using Microsoft.AspNetCore.Mvc;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;

namespace WorkReport.Controllers
{
    public class SRoleController : Controller
    {

        private readonly ISRoleService _ISRoleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public SRoleController(ISRoleService ISUserService)
        {
            _ISRoleService = ISUserService;
        }

        #region 基础增删改查

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取全部的权限分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSRole(BaseQuery baseQuery)
        {
            var rsult = _ISRoleService.GetSRole(baseQuery);
            return new JsonResult(rsult.Data);
        }

        /// <summary>
        /// 获取全部的权限列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSRoleList()
        {
            var rsult = _ISRoleService.GetSRoleList();
            return new JsonResult(rsult);
        }


        /// <summary>
        /// 编辑信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSRoleByID(int ID)
        {
            SRole sRole = _ISRoleService.Find<SRole>(ID);
            return new JsonResult(sRole);
        }

        /// <summary>
        /// 添加修改方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveSRole([FromForm] SRole sRole, [FromForm] string[] roleIDs)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (sRole != null && sRole.ID > 0)
                {
                    _ISRoleService.Update(sRole);
                }
                else
                {
                    _ISRoleService.Insert(sRole);
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
        public IActionResult DeleteSRole(int ID)
        {
            _ISRoleService.Delete<SRole>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

        #endregion

        /// <summary>
        /// 获取全部的菜单
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSRoleMenu(BaseQuery baseQuery)
        {
            var rsult = _ISRoleService.GetSRoleMenu(baseQuery);
            return new JsonResult(rsult.Data);
        }
    }
}
