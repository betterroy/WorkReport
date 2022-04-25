using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.CacheHelper;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Models.ViewModel;
using WorkReport.Repositories.Models;

namespace WorkReport.Interface.AopExtension
{
    /// <summary>
    /// 记录日志
    /// </summary>
    public class CustomAutofacSUserAop : IInterceptor
    {
        private readonly ILogger<CustomAutofacSUserAop> _logger;
        private readonly RedisHashService _RedisHashService;
        private readonly RedisStringService _RedisStringService;



        public CustomAutofacSUserAop(ILogger<CustomAutofacSUserAop> logger,
            RedisHashService redisHashService,
            RedisStringService redisStringService)
        {
            this._logger = logger;
            this._RedisHashService = redisHashService;
            this._RedisStringService = redisStringService;
        }
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            if (invocation.Method.Name.Equals("SUserLogin") && (bool)invocation.ReturnValue) //说明这里是登录，进行缓存当前用户所有菜单
            {
                SUser sUser = invocation.Arguments[2] as SUser;
                List<SRoleUser> sRoleUserList = invocation.Arguments[3] as List<SRoleUser>;
                //List<SMenuViewModel> menueViewList = invocation.Arguments[4] as List<SMenuViewModel>;
                //Dictionary<string, string> controllerList = invocation.Arguments[5] as Dictionary<string,string>;

                _logger.LogInformation(sUser.Name + "登录成功");

                if (sRoleUserList != null)
                {
                    string menuListKey = CacheKeyConstant.GetCurrentUserRoleKeyConstant(sUser.ID.ToString());   //当前用户所对应的角色
                    _RedisStringService.Set(menuListKey, sRoleUserList);
                }

                //if (controllerList != null)
                //{
                //    string menuUrKey = CacheKeyConstant.GetCurrentUserControllerKeyConstant(sUser.ID.ToString());      //当前用户的菜单Url地址  缓存的key
                //    _RedisHashService.SetRangeInHash(menuUrKey, controllerList);
                //}

                //if (menueViewList != null)
                //{
                //    string menuListKey = CacheKeyConstant.GetCurrentUserRoleMenuUrlKeyConstant(sUser.ID.ToString());   //当前用户的菜单集合  缓存的Key
                //    _RedisStringService.Set(menuListKey, menueViewList);
                //}
            }
        }
    }
}
