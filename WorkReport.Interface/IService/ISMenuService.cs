using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Models.ViewModel;
using WorkReport.Interface.IRepositories;
using WorkReport.Repositories.Models;
using WorkReport.Models.Query;
using WorkReport.Interface.AopExtension;
using Autofac.Extras.DynamicProxy;

namespace WorkReport.Interface.IService
{
    [Intercept(typeof(CustomAutofacSMenuAop))]
    public interface ISMenuService : IBaseService
    {
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public HttpResponseResult GetSMenu(BaseQuery baseQuery);

        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SMenuViewModel> GetSMenuList();

        /// <summary>
        /// 获取菜单,根据用户ID获取角色下菜单
        /// </summary>
        /// <returns></returns>
        public List<SMenuViewModel> GetSMenuList(int userId);

    }
}
