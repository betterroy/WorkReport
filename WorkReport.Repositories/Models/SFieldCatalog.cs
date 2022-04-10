using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkReport.Repositories.Models
{
    [Table("S_FieldCatalog")]
    public partial class SFieldCatalog
    {
        [Key]
        public int? ID { get; set; }
        public string CatalogName { get; set; }
        public string Field { get; set; }
        public bool IsUse { get; set; }
        public int? ParentID { get; set; }
        public int? Sort { get; set; }
    }
}
