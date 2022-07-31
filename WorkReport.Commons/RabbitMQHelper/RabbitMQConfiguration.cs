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

    public class RabbitMQExchangeQueueName
    {
        /// <summary>
        /// 写报表Routing
        /// </summary>
        public static string UReportListRouting = "UReportListRouting";
        /// <summary>
        /// 写报表交换机
        /// </summary>
        public static string UReportListExchange = "UReportListExchange";
        /// <summary>
        /// 写报表Queue
        /// </summary>
        public static string UReportListQueue = "UReportListQueue";
    }
}
