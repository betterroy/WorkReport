using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WorkReport.Commons.Api;
using WorkReport.Commons.CacheHelper;
using WorkReport.Commons.RabbitMQHelper;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.IService;
using WorkReport.Models.ViewModel;
using WorkReport.Repositories.Models;

namespace WorkReport.WebApi.Utility.Extensions
{
    public class RBMQListerExtension : RabbitListener
    {

        private readonly ILogger<RabbitListener> _logger;

        // 因为Process函数是委托回调,直接将其他Service注入的话两者不在一个scope,
        // 这里要调用其他的Service实例只能用IServiceProvider CreateScope后获取实例对象
        private readonly IServiceProvider _services;
        private readonly RedisListService _RedisListService;

        public RBMQListerExtension(IServiceProvider services, IOptions<RabbitMQConfiguration> options,
         ILogger<RabbitListener> logger, RedisListService redisListService) : base(options)
        {
            base.RouteKey = RabbitMQExchangeQueueName.UReportListRouting;
            base.QueueName = RabbitMQExchangeQueueName.UReportListQueue;
            _logger = logger;
            _services = services;
            _RedisListService = redisListService;

        }

        public override bool Process(string message)
        {
            var uReport = JsonConvert.DeserializeObject<UReport>(message);

            string uReportListKey = CacheKeyConstant.GetCurrentUReportKeyConstant();   //当前日志集合

            if (uReport == null)
            {
                // 返回false 的时候回直接驳回此消息,表示处理不了
                return false;
            }
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var _IUReportService = scope.ServiceProvider.GetRequiredService<IUReportService>();

                    SUser sUser = _IUReportService.Query<SUser>(s => s.ID == s.ID).FirstOrDefault();
                    UReportViewModel uReportViewModel = new UReportViewModel()
                    {
                        UserId = uReport.UserId,
                        CreateTime = uReport.CreateTime,
                        Content = uReport.Content,
                        ReportTime = uReport.ReportTime,
                        Name = sUser.Name
                    };

                    if (uReport.ID > 0)
                    {
                        _IUReportService.Update(uReport);

                        RemoveUReport(uReportListKey, uReport.ID);
                    }
                    else
                    {
                        _IUReportService.Insert(uReport);

                        long uReportListCount = _RedisListService.Count(uReportListKey);
                        if (uReportListCount >= 3)
                        {
                            var aa = _RedisListService.RemoveEndFromList(uReportListKey);
                        }
                    }

                    uReportViewModel.ID = uReport.ID;

                    _RedisListService.RPush(uReportListKey, JsonConvert.SerializeObject(uReportViewModel));

                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Process fail,error:{ex.Message},stackTrace:{ex.StackTrace},message:{message}");
                _logger.LogError(-1, ex, "Process fail");
                return false;
            }

        }

        /// <summary>
        /// 删除或修改时，从List中移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void RemoveUReport(string key, int? ID)
        {
            var reportList = _RedisListService.Get(key);
            var uReport = reportList.Where(r => JsonConvert.DeserializeObject<UReportViewModel>(r).ID == ID).FirstOrDefault();
            var removeCount = _RedisListService.RemoveItemFromList(key, uReport);
        }
    }
}
