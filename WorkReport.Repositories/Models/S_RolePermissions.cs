using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Repositories.Models
{
    [Table("S_RolePermissions")]
    public class SRolePermissions
    {
        public int? MenuID { get; set; }
        public int? RoleID { get; set; }
    }
}
