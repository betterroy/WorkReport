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
        /// 当前用户的菜单集合  缓存的Key
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCurrentUserMenuListKeyConstant(string userId) => $"user_{userId}_menu";

        /// <summary>
        /// 当前用户的菜单Url地址  缓存的key
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetCurrentUserMenuUrlKeyConstant(string userId) => $"user_{userId}_menuUrl";


    }
}
