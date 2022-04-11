using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Models.Query
{
    public class SFieldContentQuery:BaseQuery
    {
        public int? CatalogID { get; set; }
        public int? id { get; set; }
    }
}
