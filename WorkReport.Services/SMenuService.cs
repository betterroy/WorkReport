using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Models.ViewModel;
using WorkReport.Interface.IService;
using WorkReport.Repositories.Extend;
using WorkReport.Repositories.Models;
using WorkReport.Models.Query;
using System.Linq.Expressions;
using WorkReport.Commons.MvcResult;
using WorkReport.Commons.Extensions;
using WorkReport.Commons.Tree;

namespace WorkReport.Services
{
    public class SMenuService : BaseService, ISMenuService
    {
        public SMenuService(ICustomDbContextFactory dbContextFactory) : base(dbContextFactory)
        {

        }

        public HttpResponseResult GetSMenu(BaseQuery baseQuery)
        {

            Expression<Func<SMenu, bool>> expressionWhere = null;
            Expression<Func<SMenu, int?>> expressionOrder = c => c.ID;

            PageResult<SMenu> pageResult = QueryPage<SMenu, int?>(expressionWhere, baseQuery.limit, baseQuery.page, expressionOrder, true);
            return new HttpResponseResult() { Data = pageResult };

        }

        public List<SMenuViewModel> GetSMenuList()
        {
            List<SMenuViewModel> sMenus = RecursionMenue();
            return sMenus;
        }

        public List<SMenuViewModel> GetSMenuListByRoleID(int userId)
        {
            IQueryable<SUser> sUser = Query<SUser>(u => u.ID.Equals(userId));
            int roleId = (sUser.FirstOrDefault()?.RoleId).ToInt();
            IQueryable<SRolePermissions> sRolePermissions = Query<SRolePermissions>(r => r.RoleID.Equals(roleId));

            List<SMenuViewModel> sMenus = RecursionMenue(sRolePermissions);

            return sMenus;
        }
        private List<SMenuViewModel> RecursionMenue(IQueryable<SRolePermissions> sMenuViewModels=null)
        {
            List<SMenu>? menuQuery;

            if (sMenuViewModels == null)
            {
                menuQuery = Set<SMenu>().ToList();
            }
            else
            {
                menuQuery = Set<SMenu>().Where(m => sMenuViewModels.Any(v => v.MenuID == m.ID)).OrderBy(m => m.Sort).ToList();
            }

            List<SMenuViewModel> sMenuViewModelList = new List<SMenuViewModel>(menuQuery.Count);

            if (menuQuery != null && menuQuery.Count > 0)
            {
                foreach (SMenu menue in menuQuery)
                {
                    SMenuViewModel sMenuViewModel = new SMenuViewModel()
                    {
                        id = menue.ID,
                        title = menue.Name,
                        parentid = menue.PID,
                        Url = menue.Url,
                        Sort = menue.Sort
                    };
                    sMenuViewModelList.Add(sMenuViewModel);
                }
            }

            return TreeExtension<SMenuViewModel>.ToDo(sMenuViewModelList);
        }

    }
}
