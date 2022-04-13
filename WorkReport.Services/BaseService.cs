using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Expressions;
using WorkReport.Commons.MvcResult;
using WorkReport.Interface.IRepositories;
using WorkReport.Repositories.Extend;

namespace WorkReport.Services
{
    public class BaseService : IBaseService, IDisposable
    {
        protected DbContext Context { get; set; }

        protected ICustomDbContextFactory DbContextFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public BaseService(ICustomDbContextFactory dbContextFactory)
        {
            //Console.WriteLine($"{this.GetType().FullName}被构造....");
            DbContextFactory = dbContextFactory;
        }

        #region 仅获取Context读写对象

        public DbContext GetReadContext()
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Read);
            return this.Context;
        }
        public DbContext GetWriteContext()
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            return this.Context;
        }

        #endregion

        #region Query
        public T Find<T>(int id) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Read);
            return this.Context.Set<T>().Find(id);
        }

        /// <summary>
        /// 不应该暴露给上端使用者，尽量少用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        //[Obsolete("尽量避免使用，using 带表达式目录树的代替")]
        public IQueryable<T> Set<T>() where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Read);
            return this.Context.Set<T>();
        }

        /// <summary>
        /// 上端给条件，这里查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Read);
            if (funcWhere == null)
            {
                return this.Context.Set<T>();
            }
            else
            {
                return this.Context.Set<T>().Where<T>(funcWhere);
            }
        }

        public PageResult<T> QueryPage<T, S>(Expression<Func<T, bool>> funcWhere, int limit, int page, Expression<Func<T, S>> funcOrderby, bool isAsc = true) where T : class
        {
            var list = Set<T>();
            if (funcWhere != null)
            {
                list = list.Where<T>(funcWhere);
            }
            if (isAsc)
            {
                list = list.OrderBy(funcOrderby);
            }
            else
            {
                list = list.OrderByDescending(funcOrderby);
            }
            PageResult<T> result = new PageResult<T>()
            {
                data = list.Skip((page - 1) * limit).Take(limit).ToList(),
                page = page,
                limit = limit,
                count = list.Count()
            };
            return result;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 即使保存  不需要再Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public T Insert<T>(T t) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            this.Context.Set<T>().Add(t);
            this.Commit();//写在这里  就不需要单独commit  不写就需要
            return t;
        }

        public IEnumerable<T> Insert<T>(IEnumerable<T> tList) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            this.Context.Set<T>().AddRange(tList);
            this.Commit();//一个链接  多个sql
            return tList;
        }
        #endregion

        #region Update
        /// <summary>
        /// 是没有实现查询，直接更新的,需要Attach和State
        /// 
        /// 如果是已经在context，只能再封装一个(在具体的service)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Update<T>(T t) where T : class
        {
            if (t == null) throw new Exception("t is null");

            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            this.Context.Set<T>().Attach(t);//将数据附加到上下文，支持实体修改和新实体，重置为UnChanged
            this.Context.Entry<T>(t).State = EntityState.Modified;
            this.Commit();//保存 然后重置为UnChanged
        }

        /// <summary>
        /// 是没有实现查询，直接更新的,需要Attach和State
        /// 
        /// 如果是已经在context，只能再封装一个(在具体的service)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="params string[]">不进行更改的字段</typeparam>
        /// <param name="t"></param>
        public void Update<T>(T t, Expression<Func<T, object>> columns) where T : class
        {
            if (t == null) throw new Exception("t is null");

            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);

            var dbEntityEntry = Context.Entry(t);
            dbEntityEntry.State = EntityState.Modified;

            var excludeColumnNames = GetListByExpression.GetList(columns);
            excludeColumnNames.ForEach(e => dbEntityEntry.Property(e).IsModified = false);

            this.Commit();//保存 然后重置为UnChanged
        }

        /// <summary>
        /// 是没有实现查询，直接更新的,需要Attach和State
        /// 
        /// 如果是已经在context，只能再封装一个(在具体的service)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="params string[]">不进行更改的字段</typeparam>
        /// <param name="t"></param>
        public void Update<T>(T t, params string[] excludeColumnNames) where T : class
        {
            if (t == null) throw new Exception("t is null");

            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);

            var dbEntityEntry = Context.Entry(t);
            dbEntityEntry.State = EntityState.Modified;

            foreach (var columnName in excludeColumnNames)
            {
                //foreach (var property in dbEntityEntry.OriginalValues.Properties)
                //{
                //if (property.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                //{
                //dbEntityEntry.Property(property.Name).IsModified = false;
                //break;
                //}

                //}
                dbEntityEntry.Property(columnName).IsModified = false;
            }

            this.Commit();//保存 然后重置为UnChanged
        }

        public void Update<T>(IEnumerable<T> tList) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            foreach (var t in tList)
            {
                this.Context.Set<T>().Attach(t);
                this.Context.Entry<T>(t).State = EntityState.Modified;
            }
            this.Commit();
        }

        #endregion

        #region Delete
        /// <summary>
        /// 先附加 再删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Delete<T>(T t) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            if (t == null) throw new Exception("t is null");
            this.Context.Set<T>().Attach(t);
            this.Context.Set<T>().Remove(t);
            this.Commit();
        }

        /// <summary>
        /// 还可以增加非即时commit版本的，
        /// 做成protected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        public void Delete<T>(int Id) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            T t = this.Find<T>(Id);//也可以附加
            if (t == null) throw new Exception("t is null");
            this.Context.Set<T>().Remove(t);
            this.Commit();
        }

        public void Delete<T>(IEnumerable<T> tList) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            foreach (var t in tList)
            {
                this.Context.Set<T>().Attach(t);
            }
            this.Context.Set<T>().RemoveRange(tList);
            this.Commit();
        }
        #endregion

        #region Other
        public void Commit()
        {
            this.Context.SaveChanges();
        }

        public IQueryable<T> ExcuteQuery<T>(string sql, SqlParameter[] parameters) where T : class
        {
            return null;
        }

        public void Excute<T>(string sql, SqlParameter[] parameters) where T : class
        {
            Context = DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Write);
            IDbContextTransaction trans = null;
            try
            {
                trans = this.Context.Database.BeginTransaction();
                this.Context.Database.ExecuteSqlRaw(sql, parameters);
                trans.Commit();
            }
            catch (Exception)
            {
                if (trans != null)
                    trans.Rollback();
                throw;
            }
        }

        public virtual void Dispose()
        {
            if (this.Context != null)
            {
                this.Context.Dispose();
            }
        }

        #endregion
    }
}
