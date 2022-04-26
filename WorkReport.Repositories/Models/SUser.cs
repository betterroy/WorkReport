using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkReport.Repositories.Models
{
    [Table("S_User")]
    public partial class SUser
    {
        [Key]
        public int? ID { get; set; }
        public string UserCode { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int? UserType { get; set; }
        public string DeptId { get; set; }
        [NotMapped]
        public string RoleId { get; set; }
        public bool? Status { get; set; }
        public string Mobile { get; set; }
        public long? QQ { get; set; }
        public string WeChat { get; set; }
        public string Email { get; set; }
        public string Sex { get; set; }
    }
}
