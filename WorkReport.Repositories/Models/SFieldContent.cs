using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkReport.Repositories.Models
{
    [Table("S_FieldContent")]
    public partial class SFieldContent
    {
        [Key]
        public int? ID { get; set; }
        public int? CatalogID { get; set; }
        public int? Code { get; set; }
        public string Field { get; set; }
        public string FieldContent { get; set; }
        public bool IsUse { get; set; }
        public int? Sort { get; set; }
    }
}
