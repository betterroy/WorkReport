using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Models.Query
{
    public class SEmailQuery : BaseQuery
    {
        /// <summary>
        /// 收件人
        /// </summary>
        public string recipients { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string body { get; set; }
    }
}
