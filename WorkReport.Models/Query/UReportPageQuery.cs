
namespace WorkReport.Models.Query
{
    public class UReportPageQuery : BaseQuery
    {
        public DateTime stime { get; set; }
        public DateTime etime { get; set; }

        public int? userID { get; set; }

        public string? content { get; set; }

    }
}