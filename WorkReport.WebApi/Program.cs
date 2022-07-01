using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WorkReport.Commons.JWTHelper.Model;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Interface.Automapping;
using WorkReport.Interface.IService;
using WorkReport.Repositories;
using WorkReport.Repositories.Extend;
using WorkReport.WebApi.Utility.Extensions;
using WorkReport.WebApi.Utility.Filter;
using WorkReport.WebApi.Utility.ServiceExtension;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


{
    builder.Services.Inject();

    builder.Services.AddTransient<ICustomDbContextFactory, CustomDbContextFactory>();
    builder.Services.AddTransient<DbContext, WorkReportContext>();
    builder.Services.AddTransient<RedisStringService>();
    ///ʹ��AutoMapper
    builder.Services.AddAutoMapper(typeof(ServiceProfile));

    IConfiguration configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
.Build();
    builder.Services.Configure<DBConnectionOption>(configuration.GetSection("SqlServerConnections"));
    builder.Services.Configure<JWTTokenOptions>(configuration.GetSection("JWTTokenOptions"));

    //��Ȩ��Ȩ��ȫ��
    builder.Services.AuthenticationService(configuration);
}

builder.Services.AddControllers(option =>
{
    option.Filters.Add(new CustomExceptionFilterAttribute());
});

//����Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkReport.WebApi", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkReport.WebApi v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();    //����˭
app.UseAuthorization();     //����Ը�ʲô����ʲôȨ��

app.MapControllers();

app.Run();
