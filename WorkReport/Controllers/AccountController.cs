using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using WorkReport.Models.ViewModel;
using WorkReport.Interface.IService;
using WorkReport.Models;
using WorkReport.Repositories.Models;
using WorkReport.Utility.Filters.Attributes;
using WorkReport.Commons.EncryptHelper;
using WorkReport.Commons.Extensions;
using WorkReport.Commons.MvcResult;
using WorkReport.Commons.Api;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Commons.CacheHelper;

namespace WorkReport.Controllers
{
    public class AccountController : Controller
    {

        private readonly ISUserService _iSUserService;
        private readonly RedisStringService _RedisStringService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public AccountController(ISUserService iSUserService,
            RedisStringService redisStringService)
        {
            _iSUserService = iSUserService;
            this._RedisStringService = redisStringService;
        }

        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[AllowAnonymous]
        [CustomAllowAnonymousAttribute]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return Redirect("~/Home/Index");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 提示无权限操作
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[AllowAnonymous]
        [CustomAllowAnonymousAttribute]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[AllowAnonymous]
        [CustomAllowAnonymousAttribute]
        public IActionResult Login(string username, string password)
        {
            password = MD5Encrypt.Encrypt(password);    //对密码进行MD5加密验证。

            SUser sUser ;

            var isLogin = _iSUserService.SUserLogin(username, password, out sUser, out List<SRoleUser> sRoleUser
                //, out List<SMenuViewModel> menueViewList
                );

            if (!isLogin)
            {
                ViewBag.Message = "用户名或密码错误";
                return View();
            }
            else
            {
                CurrentUser.Value = sUser;  //存储当前登陆用户

                List<Claim> claims = new List<Claim>()
                {
                   new Claim(ClaimTypes.Sid,sUser.ID.ToString()),
                   new Claim(ClaimTypes.Name,sUser.Name),
                   new Claim("SUser",JsonConvert.SerializeObject(sUser)),
                   new Claim("id",sUser.ID.ToString())
                };
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CurrentUser"));
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),//过期时间：30分钟  
                    AllowRefresh = true
                }).Wait();
                return Redirect("/Home/Index");
            }

        }

        /// <summary>
        /// 获取当前用户的方式。
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCurrentUser()
        {
            var user = HttpContext.User;            //获取当前上下文对象
            string userId = user.FindFirst("id").Value;

            var IsAuthenticated = User.Identity.IsAuthenticated;        //是否登陆
            var name = HttpContext.User.Identity.Name;                  //ClaimTypes.Name值

            var currentClaim = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid);   //ID
            int userClaimsId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid).Value.ToInt();

            var sUserInfo = User.Claims.FirstOrDefault(u => u.Type == "SUser");

            var sUser = JsonConvert.DeserializeObject<SUserViewModel>(sUserInfo.Value);

            SUser userValue = CurrentUser.Value;

            return new JsonResult(sUser);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public IActionResult LogOut()
        {
            CurrentUser.Value = null;  //移除当前用户

            var user = HttpContext.User;            //获取当前上下文对象
            string userId = user.FindFirst("id").Value;
            List<string> keys = new List<string>();
            keys.Add(CacheKeyConstant.GetCurrentUserRoleKeyConstant(userId));   //当前用户所对应的角色
            keys.Add(CacheKeyConstant.GetCurrentUserRoleMenuUrlKeyConstant(userId));   //当前用户的菜单集合  缓存的Key
            //退出登陆时，移除当前用户菜单与权限。
            _RedisStringService.Remove(keys);

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdatePassWord([FromBody] Pass pass)
        {

            HttpResponseCode doResult = HttpResponseCode.Failed;

            var ID = HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid).Value.ToInt();
            var User = _iSUserService.Find<SUser>(ID);


            if (User != null && !string.IsNullOrEmpty(pass.password))
            {
                pass.password = MD5Encrypt.Encrypt(pass.password);    //对密码进行MD5加密验证。
                User.Password = pass.password;
                _iSUserService.Update<SUser>(User);
                doResult = HttpResponseCode.Success;
            }

            return Json(new HttpResponseResult()
            {
                Msg = "保存成功",
                Code = doResult
            });
        }
        public class Pass
        {
            public string password { get; set; }
        }


    }
}
