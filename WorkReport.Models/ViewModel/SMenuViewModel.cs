using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Extensions;
using WorkReport.Commons.Tree;

namespace WorkReport.Models.ViewModel
{
    public class SMenuViewModel : PureTreeModel
    {
        public string Url { get; set; }
        public int? Sort { get; set; }

    }
}
