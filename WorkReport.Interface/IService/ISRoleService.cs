using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Interface.IRepositories;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;

namespace WorkReport.Interface.IService
{
    public interface ISRoleService : IBaseService
    {

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <returns></returns>
        public HttpResponseResult GetSRole(BaseQuery baseQuery);

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <returns></returns>
        public List<SRole> GetSRoleList();


        #region 角色菜单

        /// <summary>
        /// 根据角色ID,获取权限菜单。前台编辑选择用
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public HttpResponseResult GetSRoleMenu(BaseQuery baseQuery, int? RoleID);

        #endregion

        #region 角色下用户

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public List<SRoleUser> GetSRoleUserByUserID(int? userID);

        #endregion
    }
}
