using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkReport.Commons.Api;

namespace WorkReport.WebApi.Utility.Filter
{
    /// <summary>
    /// 异常Filter自定义扩展
    /// </summary>
    public class CustomExceptionFilterAttribute : IExceptionFilter
    {
        /// <summary>
        /// 当异常发生的是哦户
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                context.Result = new JsonResult(new HttpResponseResult()
                {
                    Msg = context.Exception.Message
                });
            }
            context.ExceptionHandled = true;
        }
    }
}
