using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.CacheHelper
{
    public static class CacheKeyConstant
    {

        /// <summary>
        /// 当前用户拥有的角色集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCurrentUserRoleKeyConstant(string userId) => $"user_{userId}_role";

        /// <summary>
        /// 当前用户所对应的控制器目录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCurrentUserControllerKeyConstant(string roleId) => $"user_{roleId}_controller";

        /// <summary>
        /// 用户对应的菜单
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCurrentUserRoleMenuUrlKeyConstant(string roleId) => $"user_{roleId}_menu";

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCurrentUReportKeyConstant() => $"user_report";

    }
}
