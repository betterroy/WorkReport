using Microsoft.AspNetCore.Mvc;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;

namespace WorkReport.Controllers
{
    public class SDepartmentController : Controller
    {

        private readonly ISDepartmentService _ISDepartmentService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public SDepartmentController(ISDepartmentService ISUserService)
        {
            _ISDepartmentService = ISUserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取全部的部门分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSDepartment(BaseQuery baseQuery)
        {
            var rsult = _ISDepartmentService.GetSDepartment(baseQuery);
            return new JsonResult(rsult.Data);
        }

        /// <summary>
        /// 获取全部的部门列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSDepartmentList()
        {
            var rsult = _ISDepartmentService.GetSDepartmentList();
            return new JsonResult(rsult);
        }


        /// <summary>
        /// 编辑信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSDepartmentByID(int ID)
        {
            SDepartment sDepartment = _ISDepartmentService.Find<SDepartment>(ID);
            return new JsonResult(sDepartment);
        }

        /// <summary>
        /// 添加修改方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveSDepartment([FromBody] SDepartment uReport)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (uReport != null && uReport.ID > 0)
                {
                    _ISDepartmentService.Update(uReport);
                }
                else
                {
                    _ISDepartmentService.Insert(uReport);
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
        public IActionResult DeleteSDepartment(int ID)
        {
            _ISDepartmentService.Delete<SDepartment>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

    }
}
