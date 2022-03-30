using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;
using WorkReport.Models;
using WorkReport.Utility.Filters.WebHelper;

namespace WorkReport.Utility.Filters.Attributes
{
    public class CustomAuthorizeFilterAttribute : Attribute, IAuthorizationFilter
    {

        readonly ILogger _iLogger;

        public CustomAuthorizeFilterAttribute(ILogger<CustomAuthorizeFilterAttribute> iLogger)
        {
            this._iLogger = iLogger;
        }
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.ActionDescriptor.EndpointMetadata.Any(q => q is CustomAllowAnonymousAttribute))     //允许匿名访问
            {
                return;
            }

            //var currentUser = context.HttpContext.User.Claims.FirstOrDefault();

            var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;

            if (!IsAuthenticated)// 就要拦截
            {
                if (this.IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new JsonResult(new HttpResponseResult
                    {
                        Code = HttpResponseCode.Failed,
                        Msg = "没有权限"
                    });
                }
                context.Result = new RedirectResult("~/Account/Login");//短路器，只要对context.Result赋值，就不再往后执行
            }
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }

}
