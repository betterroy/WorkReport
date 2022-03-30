using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;

namespace WorkReport.Controllers
{
    public class UReportController : Controller
    {

        private readonly IUReportService _IUReportService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public UReportController(IUReportService iUReportService)
        {
            _IUReportService = iUReportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取用户的日志
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCurrentComment(UReportPageQuery query)
        {
            var rsult = _IUReportService.GetUReport(query);
            return new JsonResult(rsult);
        }

        /// <summary>
        /// 获取已填写与未填写周报的情况
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUReportWriteStatus()
        {
            return View();
        }

        /// <summary>
        /// 编辑信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUReportByID(int ID)
        {
            UReport uReport = _IUReportService.Find<UReport>(ID);
            return new JsonResult(uReport);
        }

        /// <summary>
        /// 添加修改方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveReport([FromBody] UReport uReport)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (uReport.ID > 0)
                {
                    _IUReportService.Update(uReport);
                }
                else
                {
                    _IUReportService.Insert(uReport);
                }
                doResult = HttpResponseCode.Success;
            }
            catch(Exception ex)
            {
                doResult = HttpResponseCode.Failed;
            }

            return Json(new HttpResponseResult()
            {
                Code = doResult
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteUReport(int ID)
        {
            _IUReportService.Delete<UReport>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

    }
}
