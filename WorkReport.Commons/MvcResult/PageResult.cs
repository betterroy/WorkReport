using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.MvcResult
{
    public class PageResult<T>
    {

        /// <summary>
        /// 是否成功
        /// </summary>
        public LayCodeEnum code
        {
            get
            {
                return data != null ? LayCodeEnum.Success : LayCodeEnum.Failed;
            }
        }

        /// <summary>
        /// 特指错误消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 页面索引
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 分页数量
        /// </summary>
        public int limit { get; set; }

        /// <summary>
        /// 返回数据集
        /// </summary>
        public List<T> data { get; set; }
    }

    public enum LayCodeEnum
    {
        Success = 0,
        Failed = 0
    }

}
