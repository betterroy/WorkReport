using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WorkReport.Controllers
{
    public class AjaxRequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetString(string str, int id)
        {
            IQueryCollection queryParameters = HttpContext.Request.Query;
            string qstr = queryParameters["str"];
            string qid = queryParameters["ID"];

            Console.WriteLine($"通过参数接收：str:{str},id:{id}");

            Console.WriteLine($"通过原生方式参数接收：str:{qstr},id:{qid}");
            return new JsonResult("调用成功");
        }
        [HttpPost]
        public IActionResult PostString(string str, int id)
        {
            IFormCollection queryParameters = HttpContext.Request.Form;
            string qstr = queryParameters["str"];
            string qid = queryParameters["ID"];

            Console.WriteLine($"通过参数接收：str:{str},id:{id}");

            Console.WriteLine($"通过原生方式参数接收：str:{qstr},id:{qid}");
            return new JsonResult("调用成功");
        }
        [HttpPost]
        public IActionResult PostStringFromForm([FromForm]string str, [FromForm] int id)
        {
            IFormCollection queryParameters = HttpContext.Request.Form;
            string qstr = queryParameters["str"];
            string qid = queryParameters["ID"];

            Console.WriteLine($"通过参数接收：str:{str},id:{id}");

            Console.WriteLine($"通过原生方式参数接收：str:{qstr},id:{qid}");
            return new JsonResult("调用成功");
        }



        [HttpPost]
        public IActionResult PostModelFromForm([FromForm]Person person,[FromForm]int? Id)
        {
            Console.WriteLine($"通过参数接收：str:{JsonConvert.SerializeObject(person)}");

            return new JsonResult("调用成功");
        }

        [HttpPost]
        public IActionResult PostModelFromBody([FromBody]Person person)
        {
            Console.WriteLine($"通过参数接收：str:{JsonConvert.SerializeObject(person)}");

            return new JsonResult("调用成功");
        }

        [HttpPost]
        public IActionResult PostDynamic([FromBody] dynamic person)
        {
            string Id=person.Id;
            string Name=person.Name;
            Console.WriteLine($"通过参数接收：Id:{Id},Name:{Name},str:{JsonConvert.SerializeObject(person)}");

            return new JsonResult("调用成功");
        }

        [HttpPost]
        public IActionResult PostJObject([FromBody] JObject person)
        {
            string Id = person["Id"].ToString();
            string Name = person["Name"].ToString();
            Console.WriteLine($"通过参数接收：Id:{Id},Name:{Name},str:{JsonConvert.SerializeObject(person)}");

            return new JsonResult("调用成功");
        }

    }
    public class Person
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}
