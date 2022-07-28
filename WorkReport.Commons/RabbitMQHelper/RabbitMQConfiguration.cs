using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.RabbitMQHelper
{
    public class RabbitMQConfiguration
    {
        public string RabbitHost { get; set; }
        public string RabbitUserName { get; set; }
        public string RabbitPassword { get; set; }
        public int RabbitPort { get; set; }
        
    }
}
