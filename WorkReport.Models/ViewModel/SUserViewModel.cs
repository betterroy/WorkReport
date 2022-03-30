using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Models.ViewModel
{
    public class SUserViewModel
    {
        public int? ID { get; set; }
        public string UserCode { get; set; }
        public string Name { get; set; }
        public int? UserType { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string RoleId { get; set; }
        public bool? Status { get; set; }
        public string Mobile { get; set; }
        public long? QQ { get; set; }
        public string WeChat { get; set; }
        public string Email { get; set; }
        public string Sex { get; set; }
    }
}
