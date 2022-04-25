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
    public class CustomAutofacSMenuAop : IInterceptor
    {
        private readonly ILogger<CustomAutofacSMenuAop> _logger;
        private readonly RedisStringService _RedisStringService;

        public CustomAutofacSMenuAop(ILogger<CustomAutofacSMenuAop> logger,
            RedisStringService redisStringService)
        {
            this._logger = logger;
            this._RedisStringService = redisStringService;
        }
        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method;
            if (invocation.Method.Name.Equals("GetSMenuList") && invocation.Arguments.Length > 0)
            {
                string menuListKey = CacheKeyConstant.GetCurrentUserRoleMenuUrlKeyConstant(invocation.Arguments[0].ToString());   //当前用户的菜单集合  缓存的Key
                invocation.ReturnValue = this.GetStatisticsFromRedis(menuListKey, () =>
                {
                    invocation.Proceed();
                    return (List<SMenuViewModel>)invocation.ReturnValue;
                });
            }
        }

        /// <summary>
        /// 获取Redis的值
        /// </summary>
        /// <param name="menuListKey"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private List<SMenuViewModel> GetStatisticsFromRedis(string menuListKey, Func<List<SMenuViewModel>> func)
        {
            var redisSmenu = _RedisStringService.Get<List<SMenuViewModel>>(menuListKey);
            if (redisSmenu == null) //如果Redis中无此用户菜单，则进行获取。
            {
                List<SMenuViewModel> sMenus = func.Invoke();
                _RedisStringService.Set(menuListKey, sMenus, TimeSpan.FromMinutes(30));
                return sMenus;
            }
            else
            {
                return redisSmenu;
            }
        }

    }
}
