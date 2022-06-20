using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Models.Query
{
    public class SUserQuery : BaseQuery
    {
        public string username { get; set; }
        public string password { get; set; }
        public string captcha { get; set; }
        public string tag { get; set; }
    }
}
