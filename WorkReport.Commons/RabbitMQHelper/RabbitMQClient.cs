using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WorkReport.Commons.RabbitMQHelper
{
    public class RabbitMQClient
    {

        private readonly IModel _channel;

        private readonly ILogger _logger;


        public RabbitMQClient(IOptions<RabbitMQConfiguration> options, ILogger<RabbitMQClient> logger)
        {
            try
            {
                var HostName = options.Value.RabbitHost ?? "localhost";
                var UserName = options.Value.RabbitUserName ?? "guest";
                var Password = options.Value.RabbitPassword ?? "guest";
                var Port = options.Value.RabbitPort == 0 ? 15672 : options.Value.RabbitPort;
                var factory = new ConnectionFactory()
                {
                    HostName = HostName,
                    UserName = UserName,
                    Password = Password,
                    //Port = Port
                };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQClient init fail");
            }
            _logger = logger;
        }

        public virtual void PushMessage(string routingKey, object message)
        {
            _logger.LogInformation($"PushMessage,routingKey:{routingKey}");
            _channel.QueueDeclare(queue: RabbitMQExchangeQueueName.UReportListQueue,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);


            // 把放入队列消息进行持久化
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: RabbitMQExchangeQueueName.UReportListExchange,
                                    routingKey: routingKey,
                                    //basicProperties: null,
                                    basicProperties: properties,
                                    body: body);
        }
    }
}
