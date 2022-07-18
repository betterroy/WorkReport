using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CustomAuthenticationRemarkAttribute:Attribute
    {

        /// <summary>
        /// Controller名称--不需要填写
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Controller下Action名称--不需要填写
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Controller下Action备注名称
        /// </summary>
        public string ActionRemark { get; set; }

        /// <summary>
        /// 是否显示至菜单中
        /// </summary>
        public bool IsShow { get; set; }=true;

    }

}
