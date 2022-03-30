using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkReport.Repositories.Models
{
    [Table("S_Menu")]
    public partial class SMenu
    {
        [Key]
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? PID { get; set; }
        public string Url { get; set; }
        public int? Sort { get; set; }

    }
}
