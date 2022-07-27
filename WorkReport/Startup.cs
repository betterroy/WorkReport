using Microsoft.Extensions.WebEncoders;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Autofac;
using WorkReport.Interface.Automapping;
using WorkReport.Interface.IService;
using WorkReport.Repositories;
using WorkReport.Repositories.Extend;
using WorkReport.Services;
using Microsoft.EntityFrameworkCore;
using WorkReport.Utility.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using WorkReport.Utility.Filters.Attributes;
using Microsoft.AspNetCore.Authorization;
using WorkReport.Utility.Filters.Policy;
using WorkReport.Commons.Extensions;
using System.Text.Json;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.AopExtension;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using WorkReport.Commons.EmailHelper;
using WorkReport.Commons.MongoDBHelper.Init;

namespace WorkReport
{
    public class Startup
    {

        /// <summary>
        /// asp.net core核心配置
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;

            void BindConfig()
            {
                Configuration = configuration;
            }

            BindConfig();
        }

        /// <summary>
        /// 依赖注入容器
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// 配置中心
        /// </summary>
        public IConfiguration Configuration { get; set; }

        private readonly IWebHostEnvironment _env;


        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public void ConfigureServices(IServiceCollection services)
        {
            ///解决中文编码问题
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            //全局配置Json序列化处理
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置年月日时分秒时间格式  
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            //AddRazorRuntimeCompilation支持修改源码后马上升生效
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddAutoMapper(typeof(ServiceProfile)); //注册autoMapper

            //读取配置
            IConfiguration configuration = new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.json")
                                        .AddJsonFile("middleWareConfig.json")
                                        .Build();
            services.Configure<DBConnectionOption>(configuration.GetSection("SqlServerConnections"));
            services.Configure<SendServerConfigurationEntity>(configuration.GetSection("EmailConfigInfo"));
            services.Configure<MongoDBConfig>(configuration.GetSection("MongoDBConfig"));

            services.AddTransient<ICustomDbContextFactory, CustomDbContextFactory>();
            services.AddTransient<DbContext, WorkReportContext>();
            services.Inject();  //自动注册服务


            services.AddSession();

            services.AddMvc(option =>
            {
                //option.Filters.Add(typeof(AuthorizeFilter))             //可注册全局自带过滤器
                option.Filters.Add<CustomAuthorizeFilterAttribute>();    // 增加自定义权限过滤器,判断是否登陆
                option.Filters.Add<CustomExceptionFilterAttribute>();    // 增加全局异常过滤器，记录日志
                option.Filters.Add<CustomActionFilterAttribute>();    // 增加action过滤器，记录日志
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)   //登录校验
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");//没登录跳到这个路径
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");//没权限跳到这个路径
            });

            services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();     //增加授权
            services.AddAuthorization(optins =>                                             //授权校验-此处其实未用到
            {
                //增加授权策略
                optins.AddPolicy("customPolicy", polic =>
                {
                    polic.AddRequirements(
                        new CustomAuthorizationRequirement("Policy01")  //UReportController中有案例
                                                                        // ,new CustomAuthorizationRequirement("Policy02")
                    );
                });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".less"] = "text/css";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            app.UseStaticFiles();

#if DEBUG
            app.UseDirectoryBrowser(new DirectoryBrowserOptions()   //启用目录浏览------测试时用
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = new PathString("/wwwroot")
            });
#endif

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();//检测用户是否登录
            app.UseAuthorization(); //授权，检测有没有权限，是否能够访问功能

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

            builder.RegisterType<RedisStringService>();
            builder.RegisterType<RedisHashService>();
            builder.RegisterType<RedisSetService>();
            builder.RegisterType<RedisZSetService>();
            builder.RegisterType<RedisListService>();
            builder.RegisterType(typeof(CustomAutofacSUserAop));
            builder.RegisterType(typeof(CustomAutofacSMenuAop));
            builder.RegisterType(typeof(CustomAutofacUReportAop));
            builder.RegisterType<SUserService>().As<ISUserService>().EnableClassInterceptors();
            builder.RegisterType<SMenuService>().As<ISMenuService>().EnableClassInterceptors();
            builder.RegisterType<UReportService>().As<IUReportService>().EnableClassInterceptors();
            //builder.RegisterModule(new AutofacModule());

            //系统加载时，初始化权限至redis，永不过期，权限更改时，对权限进行更新。(也可以在需要的时候判断一下没有，没有则进行添加。)

        }
    }
}
