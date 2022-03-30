using Autofac.Extensions.DependencyInjection;
using WorkReport;

//var builder = WebApplication.CreateBuilder(args);

Host.CreateDefaultBuilder(args)
.ConfigureLogging(loggbuild =>
{
    loggbuild = loggbuild.AddLog4Net("CfgFile/log4net.Config");
})
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
})
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.Build()
.Run();

