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

        public List<SMenuViewModel> GetSMenuList(int userId)
        {
            List<SMenuViewModel> sMenus = new List<SMenuViewModel>();
            RecursionMenue(sMenus, userId);
            return sMenus;
        }

        /// <summary>
        /// 根据用户ID获取全部的菜单
        /// </summary>
        /// <param name="sMenus">返回的菜单值</param>
        /// <param name="userId">待查询的用户ID</param>
        /// <param name="PID">PID</param>
        private void RecursionMenue(List<SMenuViewModel> sMenus, int userId = 0, int? PID = 0)
        {
            var menuQuery = Set<SMenu>().Where(m => PID == m.PID).OrderBy(m => m.Sort).ToList();
            if (menuQuery != null && menuQuery.Count > 0)
            {
                foreach (SMenu menue in menuQuery)
                {
                    SMenuViewModel sMenuViewModel = new SMenuViewModel()
                    {
                        ID = menue.ID,
                        Name = menue.Name,
                        PID = menue.PID,
                        Url = menue.Url,
                        Sort = menue.Sort
                    };
                    sMenus.Add(sMenuViewModel);
                    RecursionMenue(sMenuViewModel.SubSMenu, userId, sMenuViewModel.ID);
                }
            }
        }

        public List<SMenuViewModel> GetSMenuListByRoleID(int userId)
        {
            IQueryable<SUser> sUser = Query<SUser>(u => u.ID.Equals(userId));
            int roleId = (sUser.FirstOrDefault()?.RoleId).ToInt();
            IQueryable<SRolePermissions> sRolePermissions = Query<SRolePermissions>(r => r.RoleID.Equals(roleId));

            List<SMenuViewModel> sMenus = RecursionMenueByRoleID(sRolePermissions);

            return sMenus;
        }
        private List<SMenuViewModel> RecursionMenueByRoleID(IQueryable<SRolePermissions> sMenuViewModels)
        {
            var menuQuery = Set<SMenu>().Where(m => sMenuViewModels.Any(v => v.MenuID == m.ID)).OrderBy(m => m.Sort).ToList();
            List<SMenuViewModel> sMenuViewModelList = new List<SMenuViewModel>(menuQuery.Count);

            if (menuQuery != null && menuQuery.Count > 0)
            {
                foreach (SMenu menue in menuQuery)
                {
                    SMenuViewModel sMenuViewModel = new SMenuViewModel()
                    {
                        ID = menue.ID,
                        Name = menue.Name,
                        PID = menue.PID,
                        Url = menue.Url,
                        Sort = menue.Sort
                    };
                    sMenuViewModelList.Add(sMenuViewModel);
                }
            }
            return SMenuViewModelToTree.ToDo(sMenuViewModelList);
        }

    }
}
