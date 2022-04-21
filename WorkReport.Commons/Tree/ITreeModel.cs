using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Tree
{
    public interface ITreeModel
    {
        public int? id { get; set; }
        public int? parentid { get; set; }
        public string title { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<ITreeModel> children { get; set; }

        /// <summary>
        /// 添加子节点方法
        /// </summary>
        /// <param name="node"></param>
        public void AddChilrden(ITreeModel node);
    }
}
