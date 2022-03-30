using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkReport.Commons.Extensions;

namespace WorkReport.Repositories.Models
{
    [Table("S_Log")]
    public partial class SLog
    {
        public int ID { get; set; }

        [Required]
        [StringLength(36)]
        public string UserName { get; set; }

        [Required]
        [StringLength(1000)]
        public string Introduction { get; set; }

        [StringLength(4000)]
        public string Detail { get; set; }
        //public string IP { get { return IpLong.IntToIp(IP); } set { IpLong.IpToInt(value).ToString(); } }
        public string IP { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }

        public byte LogType { get; set; }

        public DateTime CreateTime { get; set; }

        public int? CreatorId { get; set; }


    }
}
