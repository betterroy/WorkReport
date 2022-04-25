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
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Commons.CacheHelper;

namespace WorkReport.Services
{
    public class SMenuService : BaseService, ISMenuService
    {
        private readonly RedisStringService _RedisStringService;
        public SMenuService(ICustomDbContextFactory dbContextFactory,
            RedisStringService redisStringService) : base(dbContextFactory)
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

        public virtual List<SMenuViewModel> GetSMenuList(int userId)
        {
            IQueryable<SRoleUser> sRoleUser = Query<SRoleUser>(u => u.UserID.Equals(userId));

            IQueryable<SRolePermissions> sRolePermissions = Query<SRolePermissions>(r => sRoleUser.Any(r => r.RoleID == r.RoleID));

            List<SMenuViewModel> sMenus = RecursionMenue(sRolePermissions);

            return sMenus;
        }
        private List<SMenuViewModel> RecursionMenue(IQueryable<SRolePermissions> sMenuViewModels = null)
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
