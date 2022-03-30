using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkReport.Repositories.Extend
{

    /// <summary>
    /// 在这里决定使用哪个DbContext，在这里决定使用哪个数据库链接字符串
    /// </summary>
    public class CustomDbContextFactory : ICustomDbContextFactory
    {
        protected DbContext _Context { get; set; }

        private DBConnectionOption _readAndWrite = null;
        public CustomDbContextFactory(DbContext context, IOptionsMonitor<DBConnectionOption> options)
        {
            _Context = context; 
            _readAndWrite = options.CurrentValue;
        }
         
        public DbContext ConnWriteOrRead(WriteAndReadEnum writeAndRead)
        {
            switch (writeAndRead)
            {
                case WriteAndReadEnum.Write:
                    //这里就指定主库连接
                    ToWrite();
                    break;
                case WriteAndReadEnum.Read:
                    //指定从库连接
                    ToRead();
                    break;
                default:
                    break;
            }
            return _Context;
        }
         
        /// <summary>
        /// 更换成主库连接
        /// </summary>
        /// <returns></returns>
        private void ToWrite()
        {
            string conn = _readAndWrite.WriteConnection;
            //_Context.Database.GetDbConnection().;
            _Context.ToWriteOrRead(conn);
        }

        private void ToRead()
        {
            string conn = string.Empty;
            {
                //随机
                int Count = _readAndWrite.ReadConnectionList.Count;
                int index = new Random().Next(0, Count);
                conn = _readAndWrite.ReadConnectionList[index];
            }
            _Context.ToWriteOrRead(conn);
        }


    }
}
