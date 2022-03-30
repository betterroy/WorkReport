using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkReport.Repositories.Models
{
    [Table("S_Department")]
    public partial class SDepartment
    {
        [Key]
        public int? ID { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
    }
}
