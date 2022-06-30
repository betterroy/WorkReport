using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ServiceStack;
using WorkReport.AuthorizationCenter.Utility;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Repositories.Extend;
using WorkReport.Repositories;
using WorkReport.Interface.Automapping;
using WorkReport.Interface.IService;
using WorkReport.Services;
using WorkReport.AuthorizationCenter.Utility.Filter;
using WorkReport.AuthorizationCenter.Utility.Extensions;
using Autofac.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region 注册服务

{
    builder.Services.AddTransient<ICustomJWTService, CustomRSSJWTervice>();
    builder.Services.AddTransient<ISUserService, SUserService>();
    builder.Services.Inject();

    builder.Services.AddTransient<ICustomDbContextFactory, CustomDbContextFactory>();
    builder.Services.AddTransient<DbContext, WorkReportContext>();
    builder.Services.AddTransient<RedisStringService>();
    ///使用AutoMapper
    builder.Services.AddAutoMapper(typeof(ServiceProfile));

    IConfiguration configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
.Build();
    builder.Services.Configure<DBConnectionOption>(configuration.GetSection("SqlServerConnections"));
    builder.Services.Configure<JWTTokenOptions>(configuration.GetSection("JWTTokenOptions"));
}


#region 中间件支持跨域请求 
//builder.Services.AddCors(option => option.AddPolicy("AllowCors", _build => _build.AllowAnyOrigin().AllowAnyMethod()));
#endregion


builder.Services.AddAutoMapper(typeof(ServiceProfile));

builder.Services.AddControllers(option =>
{
    option.Filters.Add(new CustomExceptionFilterAttribute());
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkReport.WebApi.AuthorizationCenter", Version = "v1" });
});

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkReport.WebApi.AuthorizationCenter v1"));
}

app.UseRouting();

#region 框架的cors支持跨域 
//app.UseCors("AllowCors");
#endregion

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
