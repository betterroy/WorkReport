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
        public int? PID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int? MenuType { get; set; }
        public string Target { get; set; }
        public int? Sort { get; set; }
        public byte? Status { get; set; } = 1;

    }
}
