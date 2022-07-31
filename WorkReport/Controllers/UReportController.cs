using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WorkReport.Commons.Api;
using WorkReport.Commons.Attributes;
using WorkReport.Commons.CacheHelper;
using WorkReport.Commons.MvcResult;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Models.ViewModel;
using WorkReport.Repositories.Models;
using WorkReport.Services;

namespace WorkReport.Controllers
{
    public class UReportController : Controller
    {

        private readonly IUReportService _IUReportService;
        private readonly RedisListService _RedisListService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public UReportController(IUReportService iUReportService, RedisListService redisListService)
        {
            _IUReportService = iUReportService;
            _RedisListService = redisListService;
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
        //[Authorize(policy: "customPolicy")]
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
        //[Authorize(policy: "customPolicy")]
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
        //[Authorize(policy: "customPolicy")]
        [CustomAllowAnonymousAttribute] //移除权限控制
        public IActionResult SaveReport([FromBody] UReport uReport)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            string uReportListKey = CacheKeyConstant.GetCurrentUReportKeyConstant();   //当前日志集合

            try
            {
                var sUser = _IUReportService.Query<SUser>(s => s.ID == uReport.UserId).FirstOrDefault();
                UReportViewModel uReportViewModel = new UReportViewModel()
                {
                    UserId = uReport.UserId,
                    CreateTime = uReport.CreateTime,
                    Content = uReport.Content,
                    ReportTime = uReport.ReportTime,
                    Name = sUser?.Name
                };

                if (uReport.ID > 0)
                {
                    _IUReportService.Update(uReport);

                    RemoveUReport(uReportListKey, uReport.ID);
                }
                else
                {
                    _IUReportService.Insert(uReport);

                    long uReportListCount = _RedisListService.Count(uReportListKey);
                    if (uReportListCount >= 3)
                    {
                        var aa = _RedisListService.RemoveEndFromList(uReportListKey);
                    }
                }


                uReportViewModel.ID = uReport.ID;

                _RedisListService.RPush(uReportListKey, JsonConvert.SerializeObject(uReportViewModel));

                doResult = HttpResponseCode.Success;
            }
            catch (Exception ex)
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
            string uReportListKey = CacheKeyConstant.GetCurrentUReportKeyConstant();   //当前日志集合
            RemoveUReport(uReportListKey, ID);
            _IUReportService.Delete<UReport>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

        /// <summary>
        /// 删除或修改时，从List中移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void RemoveUReport(string key, int? ID)
        {
            var reportList = _RedisListService.Get(key);
            var uReport = reportList.Where(r => JsonConvert.DeserializeObject<UReportViewModel>(r).ID == ID).FirstOrDefault();
            var removeCount = _RedisListService.RemoveItemFromList(key, uReport);
        }
    }
}
