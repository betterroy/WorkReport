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

    }
}
