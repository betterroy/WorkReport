using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using WorkReport.Commons.Api;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.IService;
using WorkReport.Repositories.Models;

namespace WorkReport.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UReportController : ControllerBase
    {

        private readonly IUReportService _IUReportService;

        private readonly RedisStringService _RedisStringService = null;

        public UReportController(RedisStringService redisStringService, IUReportService iUReportService)
        {
            _RedisStringService = redisStringService;
            _IUReportService = iUReportService;
        }

        /// <summary>
        /// 测试权限
        /// </summary>
        /// <returns></returns>
        [Route("TestRole")]
        [HttpPost]
        [Authorize]
        public IActionResult TestRole()
        {
            Claim claim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
            if (claim == null)
            {
                return new JsonResult(new HttpResponseResult() { Msg = "此次操作权限验证不通过" });
            }
            var role = claim.Value;
            return new JsonResult(role);
        }

        /// <summary>
        /// 添加修改日志
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [Route("SaveReport")]
        [HttpPost]
        [Authorize]
        public IActionResult SaveReport(UReport uReport)
        {
            HttpResponseResult httpResponseResult = new HttpResponseResult();
            httpResponseResult.Code = HttpResponseCode.Failed;

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
                httpResponseResult.Code = HttpResponseCode.Success;
            }
            catch (Exception ex)
            {
                httpResponseResult.Msg = ex.Message;
            }

            return new JsonResult(httpResponseResult);
        }
    }
}
