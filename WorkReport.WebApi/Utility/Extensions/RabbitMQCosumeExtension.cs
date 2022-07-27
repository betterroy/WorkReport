using Microsoft.Extensions.Options;
using WorkReport.Commons.RabbitMQHelper;
using WorkReport.Repositories.Models;

namespace WorkReport.WebApi.Utility.Extensions
{
    public class RabbitMQCosumeExtension
    {
        private readonly RabbitMQConsumerModel _RabbitMQConsumerModel;

        public RabbitMQCosumeExtension(IOptions<RabbitMQConsumerModel> rabbitMQConsumerModel)
        {
            _RabbitMQConsumerModel = rabbitMQConsumerModel.Value;
        }
        public async Task Invoke(HttpContext context)
        {
            await Task.Run(() =>
            {
                RabbitMQInvoker mQInvoker = new RabbitMQInvoker();
                if("".Equals(_RabbitMQConsumerModel.ExchangeName)|| "".Equals(_RabbitMQConsumerModel.QueueName))
                {
                    _RabbitMQConsumerModel.ExchangeName = "";
                    _RabbitMQConsumerModel.QueueName = "";
                }
                mQInvoker.RegistReciveAction(_RabbitMQConsumerModel, s =>
                {
                    UReport cSCommentView = Newtonsoft.Json.JsonConvert.DeserializeObject<UReport>(s);
                    Console.WriteLine($"这是接收到的日志信息：{s}");
                    return true;
                });

                Console.ReadLine();
            });

        }
    }
}
