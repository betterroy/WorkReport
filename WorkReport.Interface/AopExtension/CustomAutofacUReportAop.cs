using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceStack.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Commons.CacheHelper;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Models.ViewModel;
using WorkReport.Repositories.Models;

namespace WorkReport.Interface.AopExtension
{
    public class CustomAutofacUReportAop : IInterceptor
    {
        private readonly ILogger<CustomAutofacUReportAop> _logger;
        private readonly RedisListService _RedisListService;

        public CustomAutofacUReportAop(ILogger<CustomAutofacUReportAop> logger,
            RedisListService redisListService)
        {
            this._logger = logger;
            this._RedisListService = redisListService;
        }
        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method;
            if (invocation.Method.Name.Equals("GetUReport") && invocation.Arguments.Length > 0)
            {
                string uReportListKey = CacheKeyConstant.GetCurrentUReportKeyConstant();   //当前日志集合
                invocation.ReturnValue = this.GetStatisticsFromRedis(uReportListKey, () =>
                {
                    invocation.Proceed();
                    return (HttpResponseResult)invocation.ReturnValue;
                });
            }
        }

        /// <summary>
        /// 获取Redis的值
        /// </summary>
        /// <param name="uReportListKey"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private HttpResponseResult GetStatisticsFromRedis(string uReportListKey, Func<HttpResponseResult> func)
        {
            var redisUReport = _RedisListService.Get(uReportListKey);
            if (redisUReport == null || redisUReport.Count==0) //如果Redis中无此用户菜单，则进行获取。
            {
                var httpResponseResult = func.Invoke();

                var ureportsList = httpResponseResult.Data as List<UReportViewModel>;

                if (ureportsList == null || ureportsList.Count == 0)   //测试转换报错。
                {
                    return new HttpResponseResult() { Data = httpResponseResult };
                }

                List<string> ureportsListStr = new List<string>(ureportsList.Count);
                ureportsList.ForEach(u => ureportsListStr.Add(JsonConvert.SerializeObject(u)));
                _RedisListService.Add(uReportListKey, ureportsListStr);
                return httpResponseResult;

            }
            else
            {
                List<UReportViewModel> ureportsList = new List<UReportViewModel>(redisUReport.Count);
                redisUReport.ForEach(u => ureportsList.Add(JsonConvert.DeserializeObject<UReportViewModel>(u)));
                return new HttpResponseResult() { Data = ureportsList };
            }
        }

    }
}
