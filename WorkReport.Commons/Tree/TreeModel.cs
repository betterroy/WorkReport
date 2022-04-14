using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Tree
{
    public class TreeModel: ITrelModel
    {
        public int id { get; set; }
        public int parentid { get; set; }
        public string title { get; set; }
        public string icon { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool spread { get; set; }=false;

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool checkedBox { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<ITrelModel> children { get; set; }

        public void AddChilrden(ITrelModel node)
        {
            if (children == null)
                children = new List<ITrelModel>();
            this.children.Add(node);
        }
    }

}
