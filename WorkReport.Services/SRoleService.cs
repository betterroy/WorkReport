using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;
using WorkReport.Models.ViewModel;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Extend;
using WorkReport.Repositories.Models;
using WorkReport.Commons.Extensions;
using WorkReport.Commons.Tree;

namespace WorkReport.Services
{
    public class SRoleService : BaseService, ISRoleService
    {
        public SRoleService(ICustomDbContextFactory dbContextFactory) : base(dbContextFactory)
        {


        }


        #region 角色的基础维护
        public HttpResponseResult GetSRole(BaseQuery baseQuery)
        {

            Expression<Func<SRole, bool>> expressionWhere = null;
            Expression<Func<SRole, string>> expressionOrder = c => c.RoleCode;

            PageResult<SRole> pageResult = QueryPage<SRole, string>(expressionWhere, baseQuery.limit, baseQuery.page, expressionOrder, true);
            return new HttpResponseResult() { Data = pageResult };

        }

        public List<SRole> GetSRoleList()
        {
            var res = Set<SRole>().ToList();
            return res;
        }

        #endregion

        #region 角色菜单

        /// <summary>
        /// 根据角色ID,获取权限菜单。前台编辑选择用
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public HttpResponseResult GetSRoleMenu(BaseQuery baseQuery, int? RoleID)
        {

            Expression<Func<SMenu, bool>> expressionWhere = null;
            var SMenuResult = Query(expressionWhere).OrderBy(s => s.Sort);

            var SRolePermissionsListFromDB = Query<SRolePermissions>(r => r.RoleID == RoleID).ToList();

            List<SRoleMenuTreeViewModel> sRoleMenuTrees = new List<SRoleMenuTreeViewModel>();
            foreach (var item in SMenuResult)
            {
                SRoleMenuTreeViewModel sRoleMenuTree = new SRoleMenuTreeViewModel()
                {
                    id = item.ID.ToInt(),
                    parentid = item.PID.ToInt(),
                    spread = true,      //默认全展开
                    checkedBox = SRolePermissionsListFromDB.Any(r => r.MenuID == item.ID),      //是否选中
                    title = item.Name
                };
                sRoleMenuTrees.Add(sRoleMenuTree);
            }
            var CatalogTree = TreeExtension<SRoleMenuTreeViewModel>.ToDo(sRoleMenuTrees);
            return new HttpResponseResult() { Data = CatalogTree };

        }

        #endregion


        #region 角色下用户(SRoleUser)

        public List<SRoleUser> GetSRoleUserByUserID(int? userID)
        {
            var result = Query<SRoleUser>(r => r.UserID == userID).ToList();
            return result;
        }

        #endregion

    }
}
