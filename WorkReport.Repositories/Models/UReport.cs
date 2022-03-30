using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkReport.Repositories.Models
{
    [Table("U_Report")]
    public partial class UReport
    {
        public int? ID { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; }
        public DateTime ReportTime { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public virtual SUser User { get; set; }
    }
}
