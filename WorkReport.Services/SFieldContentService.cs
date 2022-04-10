﻿using System;
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
        public HttpResponseResult GetSFieldContent(BaseQuery baseQuery)
        {

            Expression<Func<SFieldContent, bool>> expressionWhere = s => s.CatalogID == 1;

            var Result = Query(expressionWhere).OrderBy(s => s.Sort);
            return new HttpResponseResult() { Data = Result };

        }

    }
}
