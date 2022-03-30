﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkReport.Commons;
using WorkReport.Repositories.Logger;
using WorkReport.Repositories.Models;

namespace WorkReport.Repositories
{
    public partial class WorkReportContext : DbContext
    {

        private ConfigOptions option = null;

        public WorkReportContext()
        {

        }

        public WorkReportContext(DbContextOptions<WorkReportContext> options)
            : base(options)
        {

        }


        string connectionString = string.Empty;

        public DbContext ToWriteOrRead(string conn)
        {
            connectionString = conn;
            return this;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);      //使用SqlServer的链接字符串  
            optionsBuilder.UseLazyLoadingProxies();

            #if DEBUG
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new EFLoggerProvider());  //增加打印log日志功能。
            optionsBuilder.EnableSensitiveDataLogging(true); 
            optionsBuilder.UseLoggerFactory(loggerFactory);
            #endif
            //base.OnConfiguring(optionsBuilder);

        }

        public virtual DbSet<SDepartment> SDepartments { get; set; }
        public virtual DbSet<SUser> SUsers { get; set; }
        public virtual DbSet<UReport> UReports { get; set; }
        public virtual DbSet<SMenu> SMenus { get; set; }
        public virtual DbSet<SLog> SLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
        }

    }
}