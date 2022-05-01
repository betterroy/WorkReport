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
    public class SFieldContentService : BaseService, ISFieldContentService
    {
        public SFieldContentService(ICustomDbContextFactory dbContextFactory) : base(dbContextFactory)
        {


        }

        /// <summary>
        /// 获取字典目录
        /// </summary>
        /// <returns></returns>
        public HttpResponseResult GetSFieldCatalog(BaseQuery baseQuery)
        {

            Expression<Func<SFieldCatalog, bool>> expressionWhere = s => s.IsUse == true;

            var Result = Query(expressionWhere).OrderBy(s => s.Sort);

            List<SFieldCatalogViewModel> sFieldCatalogs = new List<SFieldCatalogViewModel>();
            foreach (var item in Result)
            {
                SFieldCatalogViewModel sFieldCatalog = new SFieldCatalogViewModel()
                {
                    id = item.ID.ToInt(),
                    parentid = item.ParentID.ToInt(),
                    spread = true,
                    title = item.CatalogName+" - "+item.Field?.ToString()+" - "+item.Sort?.ToString()
                };
                sFieldCatalogs.Add(sFieldCatalog);
            }
            var CatalogTree = TreeExtension<SFieldCatalogViewModel>.ToDo(sFieldCatalogs);
            return new HttpResponseResult() { Data = CatalogTree };

        }

        /// <summary>
        /// 获取字典目录下详细字典
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <returns></returns>
        public HttpResponseResult GetSFieldContent(SFieldContentQuery baseQuery)
        {
            Expression<Func<SFieldContent, bool>> expressionWhere = s => s.CatalogID == baseQuery.CatalogID;
            Expression<Func<SFieldContent, int?>> expressionOrder = s => s.Sort;

            PageResult<SFieldContent> pageResult = QueryPage<SFieldContent, int?>(expressionWhere, baseQuery.limit, baseQuery.page, expressionOrder, true);
            return new HttpResponseResult() { Data = pageResult };
        }


        /// <summary>
        /// 根据CatalogID获取字典项
        /// </summary>
        /// <returns></returns>
        public List<SFieldContent> GetSFieldContent(int? catalogID)
        {
            var res = Query<SFieldContent>(s=>s.CatalogID== catalogID);
            return res.ToList();
        }

        /// <summary>
        /// 根据CatalogName获取字典项
        /// </summary>
        /// <returns></returns>
        public List<SFieldContent> GetSFieldContent(string catalogField)
        {
            var CatalogID = Query<SFieldCatalog>(s => s.Field== catalogField).FirstOrDefault()?.ID;
            var res = GetSFieldContent(CatalogID) ;
            return res;
        }

    }
}
