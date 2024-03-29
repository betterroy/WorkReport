﻿using Microsoft.AspNetCore.Authorization;

namespace WorkReport.Utility.Filters.Policy
{

    /// <summary>
    /// 自定义授权策略
    /// </summary>
    public class CustomAuthorizationHandler :  AuthorizationHandler<CustomAuthorizationRequirement>
    {
        public CustomAuthorizationHandler()
        {

        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizationRequirement requirement)
        {

            bool flag = false;

            //把context转换到httpConext,方便取上下文
            HttpContext httpContext = context.Resource as HttpContext;
            string path = httpContext.Request.Path;//当前访问路径，例："/Home/roleData4"

            var user = httpContext.User;
            //用户id
            string userId = user.FindFirst("id").Value;

            //登录成功时根据角色查出来这个用户的权限存到redis,这里实际是根据用户id从redis查询出来
            List<string> paths = new List<string>()
            {
                "/UReport/GetCurrentComment"
            };

            if (requirement.Name == "Policy01")
            {
                Console.WriteLine("进入自定义策略授权01...");
                ///策略1的逻辑
                if (paths.Contains(path))
                {
                    flag = true;
                }
            }

            if (flag)
            {
                context.Succeed(requirement); //验证通过了
            }

            return Task.CompletedTask; //验证不同过
        }
    }
}
