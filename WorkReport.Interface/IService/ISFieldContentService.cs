﻿using System;
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
    public interface ISFieldContentService : IBaseService
    {

        /// <summary>
        /// 获取字典目录
        /// </summary>
        /// <returns></returns>
        public HttpResponseResult GetSFieldCatalog(BaseQuery baseQuery);

        /// <summary>
        /// 获取字典目录下详细字典
        /// </summary>
        /// <returns></returns>
        public HttpResponseResult GetSFieldContent(SFieldContentQuery baseQuery);

        /// <summary>
        /// 根据CatalogID获取字典项
        /// </summary>
        /// <returns></returns>
        public List<SFieldContent> GetSFieldContent(int? catalogID);

        /// <summary>
        /// 根据CatalogName获取字典项
        /// </summary>
        /// <returns></returns>
        public List<SFieldContent> GetSFieldContent(string catalogField);



    }
}
