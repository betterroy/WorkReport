using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        
        /// <summary>
        /// 查询参数
        /// </summary>
        public string searchParams { get; set; }

        private JObject query { get; set; }

        private void getQuery()
        {
            if (query==null && !string.IsNullOrEmpty(searchParams))
            {
                query = JObject.Parse(searchParams);
            }
        }

        public JToken this[string str] { 
            get 
            {
                getQuery();

                if (query != null)
                {
                    return query[str];
                }
                return null;
            } 
        }
    }
}
