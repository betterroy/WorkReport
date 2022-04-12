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
        public IActionResult SaveSRole([FromBody] SRole uReport)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (uReport != null && uReport.ID > 0)
                {
                    _ISRoleService.Update(uReport);
                }
                else
                {
                    _ISRoleService.Insert(uReport);
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

    }
}
