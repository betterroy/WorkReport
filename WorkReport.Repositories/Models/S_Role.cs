using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Repositories.Models
{
    [Table("S_Role")]
    public class SRole
    {
        [Key]
        public int? ID { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }

}
