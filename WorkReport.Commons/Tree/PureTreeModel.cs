using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Tree
{
    public class PureTreeModel : ITreeModel
    {
        public int? id { get; set; }
        public int? parentid { get; set; }
        public string title { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<ITreeModel> children { get; set; }

        public void AddChilrden(ITreeModel node)
        {
            if (children == null)
                children = new List<ITreeModel>();
            this.children.Add(node);
        }
    }

}
