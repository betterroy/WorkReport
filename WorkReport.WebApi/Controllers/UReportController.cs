﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Channels;
using WorkReport.Commons.Api;
using WorkReport.Commons.RabbitMQHelper;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.IService;
using WorkReport.Repositories.Models;
using WorkReport.Services;

namespace WorkReport.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UReportController : ControllerBase
    {

        private readonly IUReportService _IUReportService;

        private readonly RedisStringService _RedisStringService = null;

        private readonly RabbitMQClient _RabbitMQClient = null;

        public UReportController(RedisStringService redisStringService, RabbitMQClient rabbitMQClient, IUReportService iUReportService)
        {
            _RedisStringService = redisStringService;
            _RabbitMQClient = rabbitMQClient;
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
        /// 添加修改日志至RabbitMQ
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [Route("SaveReportToRabbitMQ")]
        [HttpPost]
        //[Authorize]
        public IActionResult SaveReportToRabbitMQ(UReport uReport)
        {
            HttpResponseResult httpResponseResult = new HttpResponseResult();
            httpResponseResult.Code = HttpResponseCode.Failed;

            try
            {

                //RabbitMQInvoker rabbitMQInvoker = new RabbitMQInvoker();
                //rabbitMQInvoker.Send(RabbitMQExchangeQueueName.UReportListExchange, message);

                //自宿主启动消费消息。
                _RabbitMQClient.PushMessage(RabbitMQExchangeQueueName.UReportListRouting, uReport);

                httpResponseResult.Code = HttpResponseCode.Success;
            }
            catch (Exception ex)
            {
                httpResponseResult.Msg = ex.Message;
            }

            return new JsonResult(httpResponseResult);
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
