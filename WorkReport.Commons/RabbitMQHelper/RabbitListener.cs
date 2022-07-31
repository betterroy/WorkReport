using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace WorkReport.Commons.RabbitMQHelper
{
    public class RabbitListener : IHostedService
    {

        private readonly IConnection connection;
        private readonly IModel channel;


        public RabbitListener(IOptions<RabbitMQConfiguration> options)
        {
            try
            {
                var HostName = options.Value.RabbitHost ?? "localhost";
                var UserName = options.Value.RabbitUserName ?? "guest";
                var Password = options.Value.RabbitPassword ?? "guest";
                var Port = options.Value.RabbitPort == 0 ? 5672 : options.Value.RabbitPort;
                var factory = new ConnectionFactory()
                {
                    HostName = HostName,
                    UserName = UserName,
                    Password = Password,
                    Port = Port
                };
                this.connection = factory.CreateConnection();
                this.channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitListener init error,ex:{ex.Message}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }





        protected string RouteKey;
        protected string QueueName;

        // 处理消息的方法
        public virtual bool Process(string message)
        {
            throw new NotImplementedException();
        }

        // 注册消费者监听在这里
        public void Register()
        {
            Console.WriteLine($"RabbitListener register,routeKey:{RouteKey}");

            channel.ExchangeDeclare(exchange: RabbitMQExchangeQueueName.UReportListExchange, type: "fanout", durable: true);
            channel.QueueDeclare(queue: QueueName, durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
            channel.QueueBind(queue: QueueName,
                              exchange: RabbitMQExchangeQueueName.UReportListExchange,
                              routingKey: RouteKey);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var result = Process(message);
                if (result)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: QueueName, consumer: consumer);
        }

        public void DeRegister()
        {
            this.connection.Close();
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.connection.Close();
            return Task.CompletedTask;
        }
    }
}
