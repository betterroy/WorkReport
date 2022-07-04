using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WorkReport.AuthorizationCenter.Model;
using WorkReport.Commons.JWTHelper.Model;
using WorkReport.Models.ViewModel;
using WorkReport.Repositories.Models;

namespace WorkReport.AuthorizationCenter.Utility
{
    public class CustomRSSJWTervice : ICustomJWTService
    {
        #region Option注入
        private readonly JWTTokenOptions _JWTTokenOptions;
        public CustomRSSJWTervice(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions)
        {
            _JWTTokenOptions = jwtTokenOptions.CurrentValue;
        }
        #endregion

        public TokenOption GetToken(SUserViewModel sUser)
        {
            //string jtiCustom = Guid.NewGuid().ToString();//用来标识 Token
            Claim[] claims = new[]
            {
               new Claim(ClaimTypes.Name, sUser.Name),
               new Claim(ClaimTypes.Sid,sUser.ID.ToString())
            };

            string keyDir = Directory.GetCurrentDirectory();
            if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
            {
                keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            }

            SigningCredentials credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);

            JwtSecurityToken AccesstokenJwt = new JwtSecurityToken(
               issuer: _JWTTokenOptions.Issuer,         //签发人
               audience: _JWTTokenOptions.Audience,     //接收者
               claims: claims,                          //相关信息
               expires: DateTime.Now.AddMinutes(5),//5分钟有效期
               signingCredentials: credentials);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string accesstoken = handler.WriteToken(AccesstokenJwt);


            JwtSecurityToken RefreshtokenJwt = new JwtSecurityToken(
             issuer: _JWTTokenOptions.Issuer,
             audience: _JWTTokenOptions.Audience,
             claims: claims,
             expires: DateTime.Now.AddMinutes(20),//20分钟有效期
             signingCredentials: credentials);
            string refreshtoken = handler.WriteToken(RefreshtokenJwt);
            return new TokenOption()
            {
                AccessToken = accesstoken,
                RefreshToken = refreshtoken
            };
        }


        public TokenOption GetTwoToken(SUserViewModel sUser)
        {
            //string jtiCustom = Guid.NewGuid().ToString();//用来标识 Token
            Claim[] claims = new[]
            {
               new Claim(ClaimTypes.Name, sUser.Name),
               new Claim(ClaimTypes.Sid,sUser.ID.ToString())
            };

            string keyDir = Directory.GetCurrentDirectory();
            if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
            {
                keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            }

            SigningCredentials credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);

            JwtSecurityToken AccesstokenJwt = new JwtSecurityToken(
               issuer: _JWTTokenOptions.Issuer,
               audience: _JWTTokenOptions.Audience,
               claims: claims,
               expires: DateTime.Now.AddMinutes(5),//5分钟有效期
               signingCredentials: credentials);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string accesstoekn = handler.WriteToken(AccesstokenJwt);

            JwtSecurityToken RefreshtokenJwt = new JwtSecurityToken(
               issuer: _JWTTokenOptions.Issuer,
               audience: _JWTTokenOptions.Audience,
               claims: claims,
               expires: DateTime.Now.AddMinutes(20),//5分钟有效期
               signingCredentials: credentials);
            string refreshtoken = handler.WriteToken(RefreshtokenJwt);
            return new TokenOption()
            {
                AccessToken = accesstoekn,
                RefreshToken = refreshtoken
            };
        }
    }
}
