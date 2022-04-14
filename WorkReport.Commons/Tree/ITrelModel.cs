using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Tree
{
    public interface ITrelModel
    {
        public int id { get; set; }
        public int parentid { get; set; }
        public string title { get; set; }
        public string icon { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool spread { get; set; }

        public bool checkedBox { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<ITrelModel> children { get; set; }

        public void AddChilrden(ITrelModel node);
    }
}
