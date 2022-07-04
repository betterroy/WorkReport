using Autofac.Core;
using Microsoft.AspNetCore.Mvc;
using WorkReport.Commons.RedisHelper.Service;

namespace WorkReport.Controllers
{
    public class RedisController : Controller
    {

        private readonly RedisStringService _RedisStringService;
        private readonly RedisHashService _RedisHashService;
        private readonly RedisSetService _RedisSetService;
        private readonly RedisZSetService _RedisZSetService;
        private readonly RedisListService _RedisListService;


        /// <summary>
        /// 添加数据过期时间
        /// </summary>
        TimeSpan timeSpan = TimeSpan.FromMinutes(30);


        public RedisController(
            RedisStringService redisStringService,
            RedisHashService redisHashSetService,
            RedisSetService redisSetService,
            RedisZSetService redisZSetService,
            RedisListService redisListService)
        {
            this._RedisStringService = redisStringService;
            this._RedisHashService = redisHashSetService;
            this._RedisSetService = redisSetService;
            this._RedisZSetService = redisZSetService;
            this._RedisListService = redisListService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// string-数据类型
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_String()
        {
            _RedisStringService.Set<string>("student1", "梦的翅膀");
            Console.WriteLine(_RedisStringService.Get("student1"));

            _RedisStringService.Append("student1", "20180802");
            Console.WriteLine(_RedisStringService.Get("student1"));

            Console.WriteLine(_RedisStringService.GetAndSetValue("student1", "程序错误"));
            Console.WriteLine(_RedisStringService.Get("student1"));

            _RedisStringService.Set<string>("student2", "王", DateTime.Now.AddSeconds(5));
            Thread.Sleep(5100);
            Console.WriteLine(_RedisStringService.Get("student2"));

            _RedisStringService.Set<int>("Age", 32);
            Console.WriteLine(_RedisStringService.Incr("Age"));
            Console.WriteLine(_RedisStringService.IncrBy("Age", 3));
            Console.WriteLine(_RedisStringService.Decr("Age"));
            Console.WriteLine(_RedisStringService.DecrBy("Age", 3));
            return Json("Redis_String执行完成。可断点调试查看控制台输出信息。");
        }
        /// <summary>
        /// HashTable-数据类型
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_Hash()
{
            _RedisHashService.SetEntryInHash("student01", "Id", "001");
            _RedisHashService.SetEntryInHash("student01", "Name", "Roy");

            _RedisHashService.SetEntryInHash("student", "id", "123456");
            _RedisHashService.SetEntryInHash("student", "name", "张xx");
            _RedisHashService.SetEntryInHash("student", "remark", "60分以上");

            var keys = _RedisHashService.GetHashKeys("student");
            var values = _RedisHashService.GetHashValues("student");
            var keyValues = _RedisHashService.GetAllEntriesFromHash("student");
            Console.WriteLine(_RedisHashService.GetValueFromHash("student", "id"));

            _RedisHashService.SetEntryInHashIfNotExists("student", "name", "太子爷");
            _RedisHashService.SetEntryInHashIfNotExists("student", "description", "60分以上-2");

            Console.WriteLine(_RedisHashService.GetValueFromHash("student", "name"));
            Console.WriteLine(_RedisHashService.GetValueFromHash("student", "description"));
            _RedisHashService.RemoveEntryFromHash("student", "description");
            Console.WriteLine(_RedisHashService.GetValueFromHash("student", "description"));
            return Json("Redis_Hash执行完成。可断点调试查看控制台输出信息。");
        }
        /// <summary>
        /// Set-数据类型
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_Set()
        {
            _RedisSetService.FlushAll();//清理全部数据

            _RedisSetService.Add("advanced", "111");
            _RedisSetService.Add("advanced", "112");
            _RedisSetService.Add("advanced", "114");
            _RedisSetService.Add("advanced", "114");
            _RedisSetService.Add("advanced", "115");
            _RedisSetService.Add("advanced", "115");
            _RedisSetService.Add("advanced", "113");

            var result = _RedisSetService.GetAllItemsFromSet("advanced");

            var random = _RedisSetService.GetRandomItemFromSet("advanced");//随机获取
            _RedisSetService.GetCount("advanced");//独立的ip数
            _RedisSetService.RemoveItemFromSet("advanced", "114");

            {
                _RedisSetService.Add("begin", "111");
                _RedisSetService.Add("begin", "112");
                _RedisSetService.Add("begin", "115");

                _RedisSetService.Add("end", "111");
                _RedisSetService.Add("end", "114");
                _RedisSetService.Add("end", "113");

                var result1 = _RedisSetService.GetIntersectFromSets("begin", "end");
                var result2 = _RedisSetService.GetDifferencesFromSet("begin", "end");
                var result3 = _RedisSetService.GetUnionFromSets("begin", "end");
                //共同好友   共同关注
            }
            return Json("Redis_Set执行完成。可断点调试查看控制台输出信息。");
        }
        /// <summary>
        /// ZSet-数据类型
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_ZSet()
        {
            _RedisZSetService.FlushAll();//清理全部数据

            _RedisZSetService.Add("advanced", "1");
            _RedisZSetService.Add("advanced", "2");
            _RedisZSetService.Add("advanced", "5");
            _RedisZSetService.Add("advanced", "4");
            _RedisZSetService.Add("advanced", "7");
            _RedisZSetService.Add("advanced", "5");
            _RedisZSetService.Add("advanced", "9");

            var result1 = _RedisZSetService.GetAll("advanced");
            var result2 = _RedisZSetService.GetAllDesc("advanced");

            _RedisZSetService.AddItemToSortedSet("Sort", "BY", 123234);
            _RedisZSetService.AddItemToSortedSet("Sort", "走自己的路", 123);
            _RedisZSetService.AddItemToSortedSet("Sort", "redboy", 45);
            _RedisZSetService.AddItemToSortedSet("Sort", "大蛤蟆", 7567);
            _RedisZSetService.AddItemToSortedSet("Sort", "路人甲", 9879);
            _RedisZSetService.AddRangeToSortedSet("Sort", new List<string>() { "123", "花生", "加菲猫" }, 3232);
            var result3 = _RedisZSetService.GetAllWithScoresFromSortedSet("Sort");

            //交叉并
            return Json("Redis_ZSet执行完成。可断点调试查看控制台输出信息。");
        }
        /// <summary>
        /// List-数据类型
        /// List：保存特别快；
        ///可以通过遍历查询数据； 
        ///系统开发中，如果遇到数据存入很频繁；
        ///
        ///知乎：面向全球用户；提问者很多；数据库中保存问题的数据必然很大；
        ///----平均每天大概提问数据有1000000； 每个小时100000条记录，每一分钟 一千多数据入库；
        ///写入很频繁； 
        ///如果直接基于关系型数据来做数据存储；在不断的往数据库中写入数据，同时还有大量的用户来展示数据；展示有一栏是最新数据；任何一个用户进来都要展示最新的数据；关系型数据压力很大；
        ///可以考虑把最新写入的提问问题保存到Redis中去；
        ///任何一个人进来都需要快速的展示第一页的数据；基本上大部分都是展示标题；
        ///key---123_标题
        ///
        ///每一次有问题进入到数据库的同时也写入一个提问的数据到Redis中去，同时使用一个trim来清除历史数据；----只保存第一页的数据到Redis中去；任何一个用户进来知乎，
        ///每个人都需要快读的展示第一页的数据；直接取出Redis中的数据作为第一页数据的展示；可以做到首页部分的快速展示；
        ///
        ///关系型数据中数据量太大了；可以把数据表进行横向拆分；也可以使用list
        ///key（表的名称）----Value(数据库ID_标题信息) 
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_List()
        {
            _RedisListService.FlushAll();

            _RedisListService.Add("advanced", "Zhaoxi1234");
            _RedisListService.Add("advanced", "kevin");
            _RedisListService.Add("advanced", "大叔");
            _RedisListService.Add("advanced", "C卡");
            _RedisListService.Add("advanced", "触不到的线");
            _RedisListService.Add("advanced", "程序错误");

            var result1 = _RedisListService.Get("advanced");
            var result2 = _RedisListService.Get("advanced", 0, 3);
            //    //可以按照添加顺序自动排序；而且可以分页获取

            //    //栈
            _RedisListService.FlushAll();

            _RedisListService.Add("advanced", "Zhaoxi1234");
            _RedisListService.Add("advanced", "kevin");
            _RedisListService.Add("advanced", "大叔");
            _RedisListService.Add("advanced", "C卡");
            _RedisListService.Add("advanced", "触不到的线");
            _RedisListService.Add("advanced", "程序错误");

            for (int i = 0; i < 5; i++)
            {
                var strResult = _RedisListService.PopItemFromList("advanced");
                var result3 = _RedisListService.Get("advanced");
            }
            //    // 队列：生产者消费者模型
            _RedisListService.FlushAll();
            _RedisListService.RPush("advanced", "Zhaoxi1234");
            _RedisListService.RPush("advanced", "kevin");
            _RedisListService.RPush("advanced", "大叔");
            _RedisListService.RPush("advanced", "C卡");
            _RedisListService.RPush("advanced", "触不到的线");
            _RedisListService.RPush("advanced", "程序错误");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(_RedisListService.PopItemFromList("advanced")); //移除一个数据，同时把移除的数据返回；
                var result4 = _RedisListService.Get("advanced");
            }
            //    //分布式缓存，多服务器都可以访问到，多个生产者，多个消费者，任何产品只被消费一次
            return Json("Redis_List执行完成。可断点调试查看控制台输出信息。");
        }
    }
}
