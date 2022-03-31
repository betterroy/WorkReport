using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Repositories.Models
{
    [Table("S_Files")]
    public class SFiles
    {
        public int? ID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string MD5Code { get; set; }
        public DateTime CreateTime { get; set; }
        public int? Sort { get; set; }
        public bool? IsDel { get; set; }
    }
}
