using Chloe.Infrastructure.Interception;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.Core
{
    class DbSession : IDbSession
    {
        DbContext _dbContext;
        internal DbSession(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IDbContext DbContext { get { return this._dbContext; } }
        public IDbConnection CurrentConnection { get { return this._dbContext.AdoSession.DbConnection; } }
        /// <summary>
        /// 如果未开启事务，则返回 null
        /// </summary>
        public IDbTransaction CurrentTransaction { get { return this._dbContext.AdoSession.DbTransaction; } }
        public bool IsInTransaction { get { return this._dbContext.AdoSession.IsInTransaction; } }
        public int CommandTimeout { get { return this._dbContext.AdoSession.CommandTimeout; } set { this._dbContext.AdoSession.CommandTimeout = value; } }

        public int ExecuteNonQuery(string cmdText, params DbParam[] parameters)
        {
            return this.ExecuteNonQuery(cmdText, CommandType.Text, parameters);
        }
        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            Utils.CheckNull(cmdText, "cmdText");
            return this._dbContext.AdoSession.ExecuteNonQuery(cmdText, parameters, cmdType);
        }

        public object ExecuteScalar(string cmdText, params DbParam[] parameters)
        {
            return this.ExecuteScalar(cmdText, CommandType.Text, parameters);
        }
        public object ExecuteScalar(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            Utils.CheckNull(cmdText, "cmdText");
            return this._dbContext.AdoSession.ExecuteScalar(cmdText, parameters, cmdType);
        }

        public IDataReader ExecuteReader(string cmdText, params DbParam[] parameters)
        {
            return this.ExecuteReader(cmdText, CommandType.Text, parameters);
        }
        public IDataReader ExecuteReader(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            Utils.CheckNull(cmdText, "cmdText");
            return this._dbContext.AdoSession.ExecuteReader(cmdText, parameters, cmdType);
        }

        public void BeginTransaction()
        {
            this._dbContext.AdoSession.BeginTransaction(null);
        }
        public void BeginTransaction(IsolationLevel il)
        {
            this._dbContext.AdoSession.BeginTransaction(il);
        }
        public void CommitTransaction()
        {
            this._dbContext.AdoSession.CommitTransaction();
        }
        public void RollbackTransaction()
        {
            this._dbContext.AdoSession.RollbackTransaction();
        }

        public void AddInterceptor(IDbCommandInterceptor interceptor)
        {
            Utils.CheckNull(interceptor, "interceptor");
            this._dbContext.AdoSession.DbCommandInterceptors.Add(interceptor);
        }
        public void RemoveInterceptor(IDbCommandInterceptor interceptor)
        {
            Utils.CheckNull(interceptor, "interceptor");
            this._dbContext.AdoSession.DbCommandInterceptors.Remove(interceptor);
        }

        public void Dispose()
        {
            this._dbContext.Dispose();
        }
    }
}
