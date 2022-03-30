using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Models.ViewModel
{
    public class SMenuViewModel
    {
        public SMenuViewModel()
        {
            this.SubSMenu = new List<SMenuViewModel>();
        }
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? PID { get; set; }
        public string Url { get; set; }
        public int? Sort { get; set; }
        public List<SMenuViewModel> SubSMenu { get; set; }

    }
}
