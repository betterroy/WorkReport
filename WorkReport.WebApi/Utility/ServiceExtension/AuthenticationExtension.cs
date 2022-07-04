using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Commons.JWTHelper.Model;

namespace WorkReport.WebApi.Utility.ServiceExtension
{
    /// <summary>
    /// 鉴权授权的扩展封装
    /// </summary>
    public static class AuthenticationExtension
    {
        /// <summary>
        /// 鉴权授权的服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AuthenticationService(this IServiceCollection services, IConfiguration configuration)
        {
            #region 读取公钥 
            string path = Path.Combine(Directory.GetCurrentDirectory(), "key.public.json");
            if (!File.Exists(path))
            {
                throw new Exception("没有找到公钥");
            }
            string key = File.ReadAllText(path);
            Console.WriteLine($"KeyPath:{path}");
            var keyParams = JsonConvert.DeserializeObject<RSAParameters>(key);
            SigningCredentials credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            configuration.Bind("JWTTokenOptions", tokenOptions);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,//是否验证Issuer
                    ValidIssuer = tokenOptions.Issuer,
                    ValidateAudience = true,//是否验证Audience
                    ValidAudience = tokenOptions.Audience,//Audience 
                    ValidateLifetime = true,//是否验证失效时间 
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey 
                    IssuerSigningKey = new RsaSecurityKey(keyParams),
                    //IssuerSigningKeyValidator = (m, n, z) =>
                    // {
                    //     Console.WriteLine("This is IssuerValidator");
                    //     return true;
                    // },
                    //IssuerValidator = (m, n, z) =>
                    // {
                    //     Console.WriteLine("This is IssuerValidator");
                    //     return "http://localhost:5726";
                    // },
                    //AudienceValidator = (m, n, z) =>
                    //{
                    //    Console.WriteLine("This is AudienceValidator");
                    //    return true;
                    //    //return m != null && m.FirstOrDefault().Equals(this.Configuration["Audience"]);
                    //},//自定义校验规则，可以新登录后将之前的无效
                };

                //如果验证不通过，可以给一个时间注册一个动作，这动作就是指定返回的结果；
                options.Events = new JwtBearerEvents
                {
                    //此处为权限验证失败后触发的事件
                    OnChallenge = context =>
                    {
                        //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
                        context.HandleResponse();
                        //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换 
                        //自定义返回的数据类型
                        context.Response.ContentType = "application/json";
                        //自定义返回状态码，默认为401 我这里改成 200
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //输出Json数据结果
                        //在这里输出结果；这个结果就告诉调用者，是没有权限

                        context.Response.WriteAsync(JsonConvert.SerializeObject(new HttpResponseResult
                        {
                            Code = HttpResponseCode.Unauthorized,
                            Msg = "此次操作权限验证不通过！"
                        }));
                        return Task.FromResult(0);
                    }
                };
            });
            #endregion
        }
    }
}
