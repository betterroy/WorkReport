using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;

namespace WorkReport.Models.Query
{
    public class BaseQuery 
    {

        /// <summary>
        /// 第几页
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int limit { get; set; }

    }
}
