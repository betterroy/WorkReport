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

namespace WorkReport.Services
{
    public class SDepartmentService : BaseService, ISDepartmentService
    {
        public SDepartmentService(ICustomDbContextFactory dbContextFactory) : base(dbContextFactory)
        {


        }

        public HttpResponseResult GetSDepartment(BaseQuery baseQuery)
        {

            Expression<Func<SDepartment, bool>> expressionWhere = null ;
            Expression<Func<SDepartment, string>> expressionOrder = c => c.DeptId;

            PageResult<SDepartment> pageResult = QueryPage<SDepartment, string>(expressionWhere, baseQuery.limit, baseQuery.page, expressionOrder, true);
            return new HttpResponseResult() { Data = pageResult };

        }

        public List<SDepartment> GetSDepartmentList()
        {
            var res = Set<SDepartment>().ToList();
            return res;
        }
    }
}
