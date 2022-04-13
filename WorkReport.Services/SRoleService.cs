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
        public HttpResponseResult GetSRoleMenu(BaseQuery baseQuery)
        {

            Expression<Func<SMenu, bool>> expressionWhere = null;

            var Result = Query(expressionWhere).OrderBy(s => s.Sort);

            List<SRoleMenuTreeViewModel> sRoleMenuTrees = new List<SRoleMenuTreeViewModel>();
            foreach (var item in Result)
            {
                SRoleMenuTreeViewModel sRoleMenuTree = new SRoleMenuTreeViewModel()
                {
                    id = item.ID.ToInt(),
                    parentid = item.PID.ToInt(),
                    spread = true,
                    title = item.Name
                };
                sRoleMenuTrees.Add(sRoleMenuTree);
            }
            var CatalogTree = TreeExtension<SRoleMenuTreeViewModel>.ToDo(sRoleMenuTrees);
            return new HttpResponseResult() { Data = CatalogTree };

        }

    }
}
