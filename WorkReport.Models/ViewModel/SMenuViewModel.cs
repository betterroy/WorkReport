using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Extensions;
using WorkReport.Commons.Tree;

namespace WorkReport.Models.ViewModel
{

    /// <summary>
    /// 菜单结果对象
    /// </summary>
    public class MenusInfoResultDTO
    {
        /// <summary>
        /// 权限菜单树
        /// </summary>
        public List<SMenuViewModel> menuInfo { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        public S_LogoViewModel logoInfo { get; set; }

        /// <summary>
        /// Home
        /// </summary>
        public S_HomeViewModel homeInfo { get; set; }
    }
    public class SMenuViewModel : PureTreeModel
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public int? pid { get; set; }

        /// <summary>
        /// 节点地址
        /// </summary>
        public string href { get; set; } = "";

        /// <summary>
        /// 新开Tab方式
        /// </summary>
        public string target { get; set; } = "_self";

        /// <summary>
        /// 菜单图标样式
        /// </summary>
        public string icon { get; set; } = "";

        /// <summary>
        /// 排序
        /// </summary>
        public int? sort { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public List<SMenuViewModel> child { get; set; }

        public void AddChild(SMenuViewModel node)
        {
            if (child == null)
                child = new List<SMenuViewModel>();
            this.child.Add(node);
        }
    }
}
