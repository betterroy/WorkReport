using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using WorkReport.AuthorizationCenter.Utility;
using WorkReport.Commons.Api;
using WorkReport.Commons.EncryptHelper;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;
using WorkReport.AuthorizationCenter.Model;
using WorkReport.Models.ViewModel;

namespace WorkReport.AuthorizationCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ICustomJWTService _iJWTService = null;
        private readonly ISUserService _iSUserService = null;
        private readonly RedisStringService _RedisStringService = null;
        private readonly IMapper _iMapper;

        public AccountController(ICustomJWTService service, ISUserService iSUserService, RedisStringService redisStringService, IMapper mapper)
        {
            _iMapper = mapper;
            _iJWTService = service;
            _iSUserService = iSUserService;
            _RedisStringService = redisStringService;
        }

        /// <summary>
        /// 登录-获取Token
        /// </summary>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(SUserQuery sUserQuery)
        {
            sUserQuery.password = MD5Encrypt.Encrypt(sUserQuery.password);    //对密码进行MD5加密验证。

            var isLogin = _iSUserService.SUserLogin(sUserQuery.username, sUserQuery.password, out SUser sUser, out List<SRoleUser> sRoleUser
                //, out List<SMenuViewModel> menueViewList
                );

            if (!isLogin)
            {
                return new JsonResult(new HttpResponseResult()
                {
                    Msg = "用户名或密码错误",
                    Code = HttpResponseCode.Failed
                });
            }
            else
            {
                TokenOption option = this._iJWTService.GetToken(sUser);
                SUserViewModel user = _iMapper.Map<SUser, SUserViewModel>(sUser);

                _RedisStringService.Set(option.RefreshToken, user);
                return new JsonResult(new HttpResponseResult()
                {
                    Data = option,
                    Tag = user
                });
            }

        }
    }
}
