using System.Reflection;

namespace WorkReport.AuthorizationCenter.Utility.Extensions
{
    /// <summary>
    /// .net core自带ioc容器注入 批量注入
    /// </summary>
    public static class IOCServiceCollectionExtension
    {
        /// <summary>
        /// 接口批量注入
        /// </summary>
        /// <param name="services"></param>
        public static void Inject(this IServiceCollection services)
        {
            //通过反射加载
            Assembly asm = Assembly.Load("WorkReport.Services");
            Type[] types = asm.GetTypes().Where(
                x => x.IsPublic && x.IsClass).ToArray();

            foreach (Type type in types)
            {
                var baseInterface = type.GetInterfaces();
                if (!baseInterface.Any()) continue;
                foreach (var baseInfo in baseInterface)
                {
                    //注入每个接口
                    services.AddScoped(baseInfo, type);
                }
            }
        }
    }
}
