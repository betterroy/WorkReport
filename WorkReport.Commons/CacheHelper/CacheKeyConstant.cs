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
        /// 当前角色所对应的控制器目录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCurrentUserControllerKeyConstant(string roleId) => $"role_{roleId}_controller";

        /// <summary>
        /// 角色对应的菜单
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCurrentUserRoleMenuUrlKeyConstant(string roleId) => $"role_{roleId}_menu";

    }
}
