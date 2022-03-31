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
    public class SFilesService : BaseService, ISFilesService
    {
        public SFilesService(ICustomDbContextFactory dbContextFactory) : base(dbContextFactory)
        {


        }

        public HttpResponseResult GetSFiles(BaseQuery baseQuery)
        {

            Expression<Func<SFiles, bool>> expressionWhere = f=>f.IsDel==false ;
            Expression<Func<SFiles, int?>> expressionOrder = c => c.Sort;

            PageResult<SFiles> pageResult = QueryPage<SFiles, int?>(expressionWhere, baseQuery.limit, baseQuery.page, expressionOrder, true);
            return new HttpResponseResult() { Data = pageResult };

        }

        public List<SFiles> GetSFilesList()
        {
            var res = Set<SFiles>().ToList();
            return res;
        }
    }
}
