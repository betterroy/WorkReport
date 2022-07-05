using Autofac.Core;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.IService;
using WorkReport.Models.ViewModel;
using WorkReport.Services;
using static WorkReport.Controllers.SUserController;

namespace WorkReport.Controllers
{
    public class RedisController : Controller
    {

        private readonly RedisStringService _RedisStringService;
        private readonly RedisHashService _RedisHashService;
        private readonly RedisSetService _RedisSetService;
        private readonly RedisZSetService _RedisZSetService;
        private readonly RedisListService _RedisListService;

        private readonly ISUserService _ISUserService;

        /// <summary>
        /// 添加数据过期时间
        /// </summary>
        TimeSpan timeSpan = TimeSpan.FromMinutes(30);


        public RedisController(
            RedisStringService redisStringService,
            RedisHashService redisHashSetService,
            RedisSetService redisSetService,
            RedisZSetService redisZSetService,
            RedisListService redisListService,
            ISUserService ISUserService)
        {
            this._RedisStringService = redisStringService;
            this._RedisHashService = redisHashSetService;
            this._RedisSetService = redisSetService;
            this._RedisZSetService = redisZSetService;
            this._RedisListService = redisListService;
            this._ISUserService = ISUserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// string-数据类型--存储字符或数字，自增自减
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

            _RedisStringService.Set<string>("student2", "王", DateTime.Now.AddSeconds(2));   //设置过期时间
            Thread.Sleep(2100);
            Console.WriteLine(_RedisStringService.Get("student2"));

            _RedisStringService.Set<int>("Age", 32);
            Console.WriteLine(_RedisStringService.Incr("Age"));
            Console.WriteLine(_RedisStringService.IncrBy("Age", 3));
            Console.WriteLine(_RedisStringService.Decr("Age"));
            Console.WriteLine(_RedisStringService.DecrBy("Age", 3));
            return Json("Redis_String执行完成。可断点调试查看控制台输出信息。");
        }

        private static bool IsGoOn = true;//秒杀活动是否结束
        /// <summary>
        /// string-数据类型--超卖
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_String_OverSelling()
        {
            try
            {
                _RedisStringService.Set<int>("Stock", 20);//是库存

                List<Task> tasks = new List<Task>();
                for (int i = 0; i < 500; i++)
                {
                    int k = i;
                    tasks.Add(Task.Run(() =>//每个线程就是一个用户请求
                    {
                        using (RedisStringService _RedisStringService = new RedisStringService())
                        {
                            if (IsGoOn)
                            {
                                long index = _RedisStringService.Decr("Stock");//自减1并且返回  
                                if (index > 0)
                                {
                                    Console.WriteLine($"{k.ToString("000")}秒杀成功，秒杀商品索引为{index}");
                                    //_RedisStringService.Incr("Stock");//+1
                                    //可以分队列，去数据库操作
                                }
                                else
                                {
                                    if (IsGoOn)
                                    {
                                        IsGoOn = false;
                                    }
                                    Console.WriteLine($"{k.ToString("000")}秒杀失败，秒杀商品索引为{index}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{k.ToString("000")}秒杀停止......");
                            }
                        }
                    }));
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Redis_String_OverSelling执行完成。可断点调试查看控制台输出信息。");
        }

        /// <summary>
        /// HashTable-数据类型--存储实体
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

            bool hashExists = _RedisHashService.SetEntryInHashIfNotExists("student", "name", "太子爷");
            hashExists = _RedisHashService.SetEntryInHashIfNotExists("student", "description", "60分以上-2");

            Console.WriteLine(_RedisHashService.GetValueFromHash("student", "name"));
            Console.WriteLine(_RedisHashService.GetValueFromHash("student", "description"));
            _RedisHashService.RemoveEntryFromHash("student", "description");
            Console.WriteLine(_RedisHashService.GetValueFromHash("student", "description"));
            return Json("Redis_Hash执行完成。可断点调试查看控制台输出信息。");
        }

        /// <summary>
        /// Redis_Hash_Model-数据类型--存储实体-数据库中所有用户
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_Hash_Model()
        {
            _RedisSetService.FlushAll();//清理全部数据

            List<SUserViewModel> sUserViewModels = _ISUserService.GetSUserList();
            SUserViewModel sUserViewModel = sUserViewModels.FirstOrDefault();

            #region 使用string方式

            _RedisStringService.Set<string>($"userinfo_{sUserViewModel.ID}", Newtonsoft.Json.JsonConvert.SerializeObject(sUserViewModel));
            _RedisStringService.Set<SUserViewModel>($"userinfo_{sUserViewModel.ID}", sUserViewModel);
            var userCacheList = _RedisStringService.Get<SUserViewModel>(new List<string>() { $"userinfo_{sUserViewModel.ID}" });
            var userCache = userCacheList.FirstOrDefault();
            string sResult = _RedisStringService.Get($"userinfo_{sUserViewModel.ID}");
            userCache = Newtonsoft.Json.JsonConvert.DeserializeObject<SUserViewModel>(sResult);

            #endregion

            _RedisSetService.FlushAll();//清理全部数据

            #region 使用Hash模式--1

            //反射遍历做一下
            _RedisHashService.SetEntryInHash($"userinfo_{sUserViewModel.ID}", "UserCode", sUserViewModel.UserCode);
            _RedisHashService.SetEntryInHash($"userinfo_{sUserViewModel.ID}", "Name", sUserViewModel.Name);
            _RedisHashService.SetEntryInHash($"userinfo_{sUserViewModel.ID}", "Address", sUserViewModel.Mobile);
            _RedisHashService.SetEntryInHash($"userinfo_{sUserViewModel.ID}", "Email", sUserViewModel.Email);

            _RedisHashService.StoreAsHash<SUserViewModel>(sUserViewModel);//含ID才可以的
            var result = _RedisHashService.GetFromHash<SUserViewModel>(sUserViewModel.ID);

            sUserViewModel = sUserViewModels[2];
            _RedisHashService.SetEntryInHash($"userinfo_{sUserViewModel.ID}", "UserCode", sUserViewModel.UserCode);
            _RedisHashService.SetEntryInHash($"userinfo_{sUserViewModel.ID}", "Name", sUserViewModel.Name);
            _RedisHashService.SetEntryInHash($"userinfo_{sUserViewModel.ID}", "Address", sUserViewModel.Mobile);
            _RedisHashService.SetEntryInHash($"userinfo_{sUserViewModel.ID}", "Email", sUserViewModel.Email);

            _RedisHashService.StoreAsHash<SUserViewModel>(sUserViewModel);//含ID才可以的
            var result2 = _RedisHashService.GetFromHash<SUserViewModel>(sUserViewModel.ID);

            #endregion
            _RedisHashService.SetEntryInHash("student01", "Id", "001");
            return Json("Redis_Hash执行完成。可断点调试查看控制台输出信息。");
        }
        /// <summary>
        /// Set-数据类型--不重复的列表，求交并补集
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
            var setGetCount = _RedisSetService.GetCount("advanced");//获取总计
            _RedisSetService.RemoveItemFromSet("advanced", "114");

            {
                _RedisSetService.Add("begin", "111");
                _RedisSetService.Add("begin", "112");
                _RedisSetService.Add("begin", "115");

                _RedisSetService.Add("end", "111");
                _RedisSetService.Add("end", "114");
                _RedisSetService.Add("end", "113");

                var result1 = _RedisSetService.GetIntersectFromSets("begin", "end");    //获取交集
                var result2 = _RedisSetService.GetDifferencesFromSet("begin", "end");   //获取补集
                var result3 = _RedisSetService.GetUnionFromSets("begin", "end");        //获取并集
                //共同好友   共同关注
            }
            return Json("Redis_Set执行完成。可断点调试查看控制台输出信息。");
        }
        /// <summary>
        /// ZSet-数据类型--不重复列表，带分数
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

            _RedisListService.Add("advanced", "1234");
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
        /// <summary>
        /// Redis_List_Queue消息队列
        ///通过List写入到Redis中一部分数据；
        /// 分布式异步队列：
        /// 生产者消费者模型；生成者写入消息到Redis，消费者可以从Redis中获取消息；  可以有多个消费者来消费（把Redis中的消息给瓜分了）;
        ///
        /// 生产者（消费者和）都可以有多个；
        /// 生产者消费者模型对我们来说有什么意义？
        ///
        /// 12306买票：
        /// 1.没有生产者消费者模型：
        /// 客户端发起请求----服务端响应生成订单；---每一次请求都及时的生成订单；
        /// 用户量太大了---服务器（数据库压力很大） 很容易造成系统撑不住；
        ///
        /// 2.有生产者消费者模型
        /// 客户端发起请求----服务端不是马上到数据库去生成订单---而是把要生成订单的数据通过List存入到Redis中去（服务端相当于是生产者）；  还有另外的服务器来到Redis中去获取数据到数据库中去生成订单；
        ///
        /// 好处：1.可以增强处理能力---生产者和消费者都可以有多个
        ///       2.可以让用户降低等待时间，消费者可以慢慢消费---拉长订单生成时间--来保证系统处理能力；
        ///       3.高可用：不会因为某一个服务宕机而导致系统瘫痪
        ///       4.系统的扩展性：可以让服务在升级的过程中，独立演化；
        ///
        /// 缺陷：1.用户及时性降低了
        ///       2.复杂性更高
        ///特点：如果有多个消费者，不会重复消费同一个消息；
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_List_Queue()
        {
            _RedisListService.FlushAll();

            _RedisListService.Add("test", "这是一个学生Add1");
            _RedisListService.Add("test", "这是一个学生Add2");
            _RedisListService.Add("test", "这是一个学生Add3");

            _RedisListService.LPush("test", "这是一个学生LPush1");
            _RedisListService.LPush("test", "这是一个学生LPush2");
            _RedisListService.LPush("test", "这是一个学生LPush3");

            _RedisListService.RPush("test", "这是一个学生RPush1");
            _RedisListService.RPush("test", "这是一个学生RPush2");
            _RedisListService.RPush("test", "这是一个学生RPush3");

            List<string> stringList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                stringList.Add(string.Format($"放入任务{i}"));
            }

            _RedisListService.Add("test", stringList);
            _RedisListService.RPush("test", "");

            Console.WriteLine(_RedisListService.Count("test"));
            Console.WriteLine(_RedisListService.Count("task"));
            var list = _RedisListService.Get("test");
            list = _RedisListService.Get("task", 2, 4);

            Task consumeMessageTask = Task.Run(() =>
            {
                int i = 1;
                while (true)
                {
                    string message = _RedisListService.DequeueItemFromList("test");
                    if (string.IsNullOrEmpty(message)) break;
                    Console.WriteLine($"消息{i}：{message}");
                    Thread.Sleep(200);
                    i++;
                }
            });
            Task.WaitAll(consumeMessageTask);
            return Json("Redis_List_Queue执行完成。可断点调试查看控制台输出信息。");
        }
        
        /// <summary>
        /// Redis_List_Subscription发布订阅
        /// 发布订阅模型：观察者模式；总部发送一个消息，所以子公司都要收到。
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_List_Subscription()
        {
            _RedisListService.FlushAll();

            Task.Run(() =>
            {
                using (RedisListService _RedisListService1 = new RedisListService())
                {
                    _RedisListService1.Subscribe("info1", (c, message, iRedisSubscription) =>
                     {
                         Console.WriteLine($"注册{1}{c}:{message}，用户1---&&&&&&&&&&&&&Dosomething else");
                         if (message.Equals("exit"))
                             iRedisSubscription.UnSubscribeFromChannels("info1");
                     });//blocking
                }
            });
            Task.Run(() =>
            {
                using (RedisListService _RedisListService2 = new RedisListService())
                {
                    _RedisListService2.Subscribe("info1", (c, message, iRedisSubscription) =>
                    {
                        Console.WriteLine($"注册{2}{c}:{message}，用户2---&&&&&&&&&&&&&Dosomething else");
                        if (message.Equals("exit"))
                            iRedisSubscription.UnSubscribeFromChannels("info1");
                    });
                }
            });
            Task.Run(() =>
            {
                using (RedisListService _RedisListService3 = new RedisListService())
                {
                    _RedisListService3.Subscribe("info2", (c, message, iRedisSubscription) =>
                    {
                        Console.WriteLine($"注册{2}{c}:{message}，用户3---&&&&&&&&&&&&&Dosomething else");
                        if (message.Equals("exit"))
                            iRedisSubscription.UnSubscribeFromChannels("info2");
                    });
                }
            });

            using (RedisListService _RedisListService4 = new RedisListService())
            {
                _RedisListService4.Publish("info1", "info1123");
                _RedisListService4.Publish("info1", "info1234");
                _RedisListService4.Publish("info1", "info1345");
                _RedisListService4.Publish("info1", "info1456");
                _RedisListService4.Publish("info2", "info2123");
                _RedisListService4.Publish("info2", "info2234");
                _RedisListService4.Publish("info2", "info2345");
                _RedisListService4.Publish("info2", "info2456");
                Console.WriteLine("**********************************************");
                _RedisListService4.Publish("info1", "exit");
                _RedisListService4.Publish("info1", "123info1");
                _RedisListService4.Publish("info1", "234info1");
                _RedisListService4.Publish("info2", "exit");
                _RedisListService4.Publish("info2", "123info2");
            }
            return Json("Redis_List_Subscription执行完成。可断点调试查看控制台输出信息。");
        }
        
        /// <summary>
        /// Redis_List_Subscription发布订阅
        /// 发布订阅模型：观察者模式；总部发送一个消息，所以子公司都要收到。
        /// </summary>
        /// <returns></returns>
        public IActionResult Redis_List_Page()
        {
            _RedisListService.FlushAll();
            for (int i = 0; i < 50; i++)
            {
                _RedisListService.RPush("newBlog", $"数值{i}");
            }

            _RedisListService.TrimList("newBlog", 0, 200);//一个list最多2的32次方-1
            var blogs1 = _RedisListService.Get("newBlog", 0, 9);
            //后面的页也就是在这里获取
            var blogs2 = _RedisListService.Get("newBlog", 10, 19);
            var blogs3 = _RedisListService.Get("newBlog", 20, 29);
            return Json("Redis_List_Page执行完成。可断点调试查看控制台输出信息。");
        }
    }
}
