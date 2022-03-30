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

namespace WorkReport.Interface.IService
{
    public interface ISMenuService : IBaseService
    {
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public HttpResponseResult GetSMenu(BaseQuery baseQuery);
        public List<SMenuViewModel> GetSMenuList(int userId);

    }
}
