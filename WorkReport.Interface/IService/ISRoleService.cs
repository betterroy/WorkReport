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


    }
}
