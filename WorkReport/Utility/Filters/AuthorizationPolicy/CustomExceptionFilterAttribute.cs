using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;

namespace WorkReport.Utility.Filters.AuthorizationPolicy
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public class CustomExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;

        readonly ILogger _iLogger;

        public CustomExceptionFilterAttribute(IModelMetadataProvider modelMetadataProvider, ILogger<CustomExceptionFilterAttribute> iLogger)
        {
            this._modelMetadataProvider = modelMetadataProvider;
            this._iLogger = iLogger;
        }

        /// <summary>
        /// 当异常发生的时候就触发
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            //如果异常没有被处理---就处理
            if (!context.ExceptionHandled)
            {

                string actionName = context.ActionDescriptor.RouteValues["action"];
                string controllerName = context.ActionDescriptor.RouteValues["controller"];

                _iLogger.LogError($"控制器：{controllerName}；方法：{actionName}；发生异常： {context.Exception.Message}");

                if (this.IsAjaxRequest(context.HttpContext.Request))//header看看是不是XMLHttpRequest
                {
                    context.Result = new JsonResult(new HttpResponseResult
                    {
                        Code = HttpResponseCode.Failed,
                        Msg = context.Exception.Message
                    });//中断式---请求到这里结束了，不再继续Action
                }
                else
                {
                    //var result = new ViewResult { ViewName = "~/Error/Error.cshtml" };
                    //result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                    //result.ViewData.Add("Exception", context.Exception);
                    //context.Result = result; //断路器---只要对Result赋值--就不继续往后了；

                    context.Result = new RedirectResult("/Error/500");
                }
                context.ExceptionHandled = true;
            }
        }
        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
}
