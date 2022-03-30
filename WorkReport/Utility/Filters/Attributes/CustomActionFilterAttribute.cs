using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using WorkReport.Commons.Extensions;
using WorkReport.Interface.IService;
using WorkReport.Models.Enum;
using WorkReport.Repositories.Models;

namespace WorkReport.Utility.Filters.Attributes
{
    public class CustomActionFilterAttribute : Attribute, IActionFilter
    {
        private readonly ISUserService _ISUserService = null;

        private readonly ILogger _iLogger;

        public CustomActionFilterAttribute(ISUserService _ISUserService, ILogger<CustomActionFilterAttribute> iLogger)
        {
            this._iLogger = iLogger;
            this._ISUserService = _ISUserService;
        }

        /// <summary>
        /// 增加访问记录日志。
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public string JsonQuery { get; set; }
        public string JsonForm { get; set; }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            JsonQuery = JsonConvert.SerializeObject(context.ActionArguments);
            //JsonForm = JsonConvert.SerializeObject(context.ActionArguments);

            var request = context.HttpContext.Request;

            string actionName = context.ActionDescriptor.RouteValues["action"];
            string controllerName = context.ActionDescriptor.RouteValues["controller"];

            if (!(actionName == "Login" && controllerName == "Account") && !(actionName == "Home" && controllerName == "Redirect"))    //不为登陆则记录
            {

                var sid = context.HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid);
                int? creatorId = sid?.Value.ToInt();

                SLog sLog = new SLog()
                {
                    UserName = context.HttpContext.User.Identity.Name,
                    Introduction = "业务日志",
                    Detail = $"参数：{JsonQuery}；",
                    IP = request.Host.Value,
                    ActionName = actionName,
                    ControllerName = controllerName,
                    LogType = (int)LogTypeEnum.Info,
                    CreateTime = DateTime.Now,
                    CreatorId = creatorId
                };

                _ISUserService.Insert(sLog);
            }
        }
    }
}
