using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ZhaoXi.LiveBackgroundManagement.DataAccessEFCore.Models;
using ZhaoXi.LiveBackgroundManagement.Models.ViewModel;
using ZhaoXi.LiveReceptionManagement.AuthorizationCenter.Model;

namespace WorkReport.AuthorizationCenter.Utility
{
    //public class CustomHSJWTService : ICustomJWTService
    //{
    //    #region Option注入
    //    private readonly JWTTokenOptions _JWTTokenOptions;
    //    public CustomHSJWTService(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions)
    //    {
    //        this._JWTTokenOptions = jwtTokenOptions.CurrentValue;
    //    }
    //    #endregion
    //    /// <summary>
    //    /// 用户登录成功以后，用来生成Token的方法
    //    /// </summary>
    //    /// <param name="cSUser"></param>
    //    /// <returns></returns>
    //    public string GetToken(CSUser cSUser)
    //    {
    //        #region 有效载荷，大家可以自己写，爱写多少写多少；尽量避免敏感信息
    //        var claims = new[]
    //       {
    //           new Claim(ClaimTypes.Name, cSUser.Name),
    //           new Claim(ClaimTypes.Sid,cSUser.Id.ToString()),
    //        };

    //        //需要加密：需要加密key:
    //        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTTokenOptions.SecurityKey));

    //        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //        JwtSecurityToken token = new JwtSecurityToken(
    //         issuer: _JWTTokenOptions.Issuer,
    //         audience: _JWTTokenOptions.Audience,
    //         claims: claims,
    //         expires: DateTime.Now.AddMinutes(5),//5分钟有效期
    //         signingCredentials: creds);
    //        string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
    //        return returnToken;
    //        #endregion

    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="cSUser"></param>
    //    /// <returns></returns>
    //    public TokenOption GetTwoToken(CSUserViewModel cSUser)
    //    {
    //        throw new Exception("建设中") ;
    //    }
    //}
}
