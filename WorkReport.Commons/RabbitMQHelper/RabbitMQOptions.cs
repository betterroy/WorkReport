using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.RabbitMQHelper
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class RabbitMQConsumerModel
    {
        /// <summary>
        /// 生产者指定，交换机
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 自己起的名字
        /// </summary>
        public string QueueName { get; set; }
    }

    public class RabbitMQExchangeQueueName
    {
        /// <summary>
        /// 写报表交换机
        /// </summary>
        public static string UReportListExchange = "UReportListExchange";
    }
}
