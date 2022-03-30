using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Models.ViewModel
{
    public class UReportViewModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; }
        public DateTime? CreateTime { get; set; }
    }

    public class UReportUserViewModel
    {
        public DateTime ReportTime { get; set; }

        public string Department { get; set; }

        public List<string> UserHadWrite { get; set; }
        public List<string> UserUnWrite { get; set; }
    }


}
