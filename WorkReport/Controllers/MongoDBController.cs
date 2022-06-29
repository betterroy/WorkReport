using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WorkReport.Commons.MongoDBHelper.Init;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.IService;

namespace WorkReport.Controllers
{
    public class MongoDBController : Controller
    {

        private readonly MongoDBConfig _mongoDBConfig;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public MongoDBController(IOptionsMonitor<MongoDBConfig> mongoDBConfig)
        {
            _mongoDBConfig = mongoDBConfig.CurrentValue;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
