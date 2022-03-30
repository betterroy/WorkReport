using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Models.ViewModel
{
    public class DepartmentAndUsersViewModel
    {
        public SDepartmentViewModel SDepartment { get; set; }

        public List<SUserViewModel> Users { get; set; }
    }
}
