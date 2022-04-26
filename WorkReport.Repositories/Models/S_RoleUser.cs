using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Repositories.Models
{
    [Table("S_RoleUser")]
    public class SRoleUser
    {
        public int? RoleID { get; set; }
        public int? UserID { get; set; }
    }
}
