using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Models.ViewModel;
using WorkReport.Interface.IRepositories;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;
using WorkReport.Interface.AopExtension;
using Autofac.Extras.DynamicProxy;

namespace WorkReport.Interface.IService
{
    [Intercept(typeof(CustomAutofacCacheAop))]
    public interface ISUserService : IBaseService
    {
        /// <summary>
        /// 获取全部用户分页
        /// </summary>
        /// <returns></returns>
        public HttpResponseResult GetSUser(BaseQuery baseQuery);

        /// <summary>
        /// 获取当前用户分页数据带部门
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <returns></returns>
        public HttpResponseResult GetSUserAndDepartment(BaseQuery baseQuery);

        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <returns></returns>
        public List<SUserViewModel> GetSUserList();

        #region 登录

        /// <summary>
        /// 登录后获取当前用户的权限
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sysUser"></param>
        /// <param name="menuUrlDictionary"></param>
        /// <param name="menueViewList"></param>
        /// <returns></returns>
        public bool SUserLogin(string username, string password, out SUser sysUser, out List<SRoleUser> sRoleUser);


        /// <summary>
        /// 获取部门与及归属用户
        /// </summary>
        /// <returns></returns>
        public List<DepartmentAndUsersViewModel> GetUserOfDepartment();


        #endregion
    }
}
