using Chloe.Core;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using Chloe.Infrastructure.Interception;
using Chloe.InternalExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.Core
{
    class InternalAdoSession : IDisposable
    {
        IDbConnection _dbConnection;
        IDbTransaction _dbTransaction;
        bool _isInTransaction;
        int _commandTimeout = 30;/* seconds */

        List<IDbCommandInterceptor> _dbCommandInterceptors;

        bool _disposed = false;

        public InternalAdoSession(IDbConnection conn)
        {
            this._dbConnection = conn;
        }

        public IDbConnection DbConnection { get { return this._dbConnection; } }
        /// <summary>
        /// 如果未开启事务，则返回 null
        /// </summary>
        public IDbTransaction DbTransaction { get { return this._dbTransaction; } }
        public bool IsInTransaction { get { return this._isInTransaction; } }
        public int CommandTimeout { get { return this._commandTimeout; } set { this._commandTimeout = value; } }
        public List<IDbCommandInterceptor> DbCommandInterceptors
        {
            get
            {
                if (this._dbCommandInterceptors == null)
                    this._dbCommandInterceptors = new List<IDbCommandInterceptor>();

                return this._dbCommandInterceptors;
            }
        }


        void Activate()
        {
            this.CheckDisposed();

            if (this._dbConnection.State == ConnectionState.Broken)
            {
                this._dbConnection.Close();
            }

            if (this._dbConnection.State == ConnectionState.Closed)
            {
                this._dbConnection.Open();
            }
        }

        /* 表示一次查询完成。在事务中的话不关闭连接，交给 CommitTransaction() 或者 RollbackTransaction() 控制，否则调用 IDbConnection.Close() 关闭连接 */
        public void Complete()
        {
            if (!this._isInTransaction)
            {
                if (this._dbConnection.State == ConnectionState.Open)
                {
                    this._dbConnection.Close();
                }
            }
        }

        public void BeginTransaction(IsolationLevel? il)
        {
            this.Activate();

            if (il == null)
                this._dbTransaction = this._dbConnection.BeginTransaction();
            else
                this._dbTransaction = this._dbConnection.BeginTransaction(il.Value);

            this._isInTransaction = true;
        }
        public void CommitTransaction()
        {
            if (!this._isInTransaction)
            {
                throw new ChloeException("Current session does not open a transaction.");
            }
            this._dbTransaction.Commit();
            this.ReleaseTransaction();
            this.Complete();
        }
        public void RollbackTransaction()
        {
            if (!this._isInTransaction)
            {
                throw new ChloeException("Current session does not open a transaction.");
            }
            this._dbTransaction.Rollback();
            this.ReleaseTransaction();
            this.Complete();
        }

        public IDataReader ExecuteReader(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            return this.ExecuteReader(cmdText, parameters, cmdType, CommandBehavior.Default);
        }
        public IDataReader ExecuteReader(string cmdText, DbParam[] parameters, CommandType cmdType, CommandBehavior behavior)
        {
            this.CheckDisposed();

            List<OutputParameter> outputParameters;
            IDbCommand cmd = this.PrepareCommand(cmdText, parameters, cmdType, out outputParameters);

            DbCommandInterceptionContext<IDataReader> dbCommandInterceptionContext = new DbCommandInterceptionContext<IDataReader>();
            IDbCommandInterceptor[] globalInterceptors = DbInterception.GetInterceptors();

            this.Activate();
            this.OnReaderExecuting(cmd, dbCommandInterceptionContext, globalInterceptors);

            IDataReader reader;
            try
            {
                reader = new InternalDataReader(this, cmd.ExecuteReader(behavior), cmd, outputParameters);
            }
            catch (Exception ex)
            {
                dbCommandInterceptionContext.Exception = ex;
                this.OnReaderExecuted(cmd, dbCommandInterceptionContext, globalInterceptors);

                throw WrapException(ex);
            }

            dbCommandInterceptionContext.Result = reader;
            this.OnReaderExecuted(cmd, dbCommandInterceptionContext, globalInterceptors);

            return reader;
        }
        public int ExecuteNonQuery(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            this.CheckDisposed();

            IDbCommand cmd = null;
            try
            {
                List<OutputParameter> outputParameters;
                cmd = this.PrepareCommand(cmdText, parameters, cmdType, out outputParameters);

                DbCommandInterceptionContext<int> dbCommandInterceptionContext = new DbCommandInterceptionContext<int>();
                IDbCommandInterceptor[] globalInterceptors = DbInterception.GetInterceptors();

                this.Activate();
                this.OnNonQueryExecuting(cmd, dbCommandInterceptionContext, globalInterceptors);

                int rowsAffected;
                try
                {
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    dbCommandInterceptionContext.Exception = ex;
                    this.OnNonQueryExecuted(cmd, dbCommandInterceptionContext, globalInterceptors);

                    throw WrapException(ex);
                }

                dbCommandInterceptionContext.Result = rowsAffected;
                this.OnNonQueryExecuted(cmd, dbCommandInterceptionContext, globalInterceptors);
                OutputParameter.CallMapValue(outputParameters);

                return rowsAffected;
            }
            finally
            {
                this.Complete();
                if (cmd != null)
                    cmd.Dispose();
            }
        }
        public object ExecuteScalar(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            this.CheckDisposed();

            IDbCommand cmd = null;
            try
            {
                List<OutputParameter> outputParameters;
                cmd = this.PrepareCommand(cmdText, parameters, cmdType, out outputParameters);

                DbCommandInterceptionContext<object> dbCommandInterceptionContext = new DbCommandInterceptionContext<object>();
                IDbCommandInterceptor[] globalInterceptors = DbInterception.GetInterceptors();

                this.Activate();
                this.OnScalarExecuting(cmd, dbCommandInterceptionContext, globalInterceptors);

                object ret;
                try
                {
                    ret = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    dbCommandInterceptionContext.Exception = ex;
                    this.OnScalarExecuted(cmd, dbCommandInterceptionContext, globalInterceptors);

                    throw WrapException(ex);
                }

                dbCommandInterceptionContext.Result = ret;
                this.OnScalarExecuted(cmd, dbCommandInterceptionContext, globalInterceptors);
                OutputParameter.CallMapValue(outputParameters);

                return ret;
            }
            finally
            {
                this.Complete();
                if (cmd != null)
                    cmd.Dispose();
            }
        }


        public void Dispose()
        {
            if (this._disposed)
                return;

            if (this._dbTransaction != null)
            {
                if (this._isInTransaction)
                {
                    try
                    {
                        this._dbTransaction.Rollback();
                    }
                    catch
                    {
                    }
                }

                this.ReleaseTransaction();
            }

            if (this._dbConnection != null)
            {
                this._dbConnection.Dispose();
            }

            this._disposed = true;
        }

        IDbCommand PrepareCommand(string cmdText, DbParam[] parameters, CommandType cmdType, out List<OutputParameter> outputParameters)
        {
            outputParameters = null;

            IDbCommand cmd = this._dbConnection.CreateCommand();

            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            cmd.CommandTimeout = this._commandTimeout;
            if (this.IsInTransaction)
                cmd.Transaction = this._dbTransaction;

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    DbParam param = parameters[i];
                    if (param == null)
                        continue;

                    if (param.ExplicitParameter != null)/* 如果存在创建好了的 IDbDataParameter，则直接用它。同时也忽视了 DbParam 的其他属性 */
                    {
                        cmd.Parameters.Add(param.ExplicitParameter);
                        continue;
                    }

                    IDbDataParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = param.Name;

                    Type parameterType;
                    if (param.Value == null || param.Value == DBNull.Value)
                    {
                        parameter.Value = DBNull.Value;
                        parameterType = param.Type;
                    }
                    else
                    {
                        parameterType = param.Value.GetType();
                        if (parameterType.IsEnum)
                        {
                            parameterType = Enum.GetUnderlyingType(parameterType);
                            parameter.Value = Convert.ChangeType(param.Value, parameterType);
                        }
                        else
                        {
                            parameter.Value = param.Value;
                        }
                    }

                    if (param.Precision != null)
                        parameter.Precision = param.Precision.Value;

                    if (param.Scale != null)
                        parameter.Scale = param.Scale.Value;

                    if (param.Size != null)
                        parameter.Size = param.Size.Value;

                    if (param.DbType != null)
                        parameter.DbType = param.DbType.Value;
                    else
                    {
                        DbType? dbType = MappingTypeSystem.GetDbType(parameterType);
                        if (dbType != null)
                            parameter.DbType = dbType.Value;
                    }

                    const int defaultSizeOfStringOutputParameter = 4000;/* 当一个 string 类型输出参数未显示指定 Size 时使用的默认大小。如果有需要更大或者该值不足以满足需求，需显示指定 DbParam.Size 值 */

                    OutputParameter outputParameter = null;
                    if (param.Direction == ParamDirection.Input)
                    {
                        parameter.Direction = ParameterDirection.Input;
                    }
                    else if (param.Direction == ParamDirection.Output)
                    {
                        parameter.Direction = ParameterDirection.Output;
                        param.Value = null;
                        if (param.Size == null && param.Type == UtilConstants.TypeOfString)
                        {
                            parameter.Size = defaultSizeOfStringOutputParameter;
                        }
                        outputParameter = new OutputParameter(param, parameter);
                    }
                    else if (param.Direction == ParamDirection.InputOutput)
                    {
                        parameter.Direction = ParameterDirection.InputOutput;
                        if (param.Size == null && param.Type == UtilConstants.TypeOfString)
                        {
                            parameter.Size = defaultSizeOfStringOutputParameter;
                        }
                        outputParameter = new OutputParameter(param, parameter);
                    }
                    else
                        throw new NotSupportedException(string.Format("ParamDirection '{0}' is not supported.", param.Direction));

                    cmd.Parameters.Add(parameter);

                    if (outputParameter != null)
                    {
                        if (outputParameters == null)
                            outputParameters = new List<OutputParameter>();
                        outputParameters.Add(outputParameter);
                    }
                }
            }

            return cmd;
        }


        #region DbInterception
        void OnReaderExecuting(IDbCommand cmd, DbCommandInterceptionContext<IDataReader> dbCommandInterceptionContext, IDbCommandInterceptor[] globalInterceptors)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.ReaderExecuting(cmd, dbCommandInterceptionContext);
            }, globalInterceptors);
        }
        void OnReaderExecuted(IDbCommand cmd, DbCommandInterceptionContext<IDataReader> dbCommandInterceptionContext, IDbCommandInterceptor[] globalInterceptors)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.ReaderExecuted(cmd, dbCommandInterceptionContext);
            }, globalInterceptors);
        }
        void OnNonQueryExecuting(IDbCommand cmd, DbCommandInterceptionContext<int> dbCommandInterceptionContext, IDbCommandInterceptor[] globalInterceptors)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.NonQueryExecuting(cmd, dbCommandInterceptionContext);
            }, globalInterceptors);
        }
        void OnNonQueryExecuted(IDbCommand cmd, DbCommandInterceptionContext<int> dbCommandInterceptionContext, IDbCommandInterceptor[] globalInterceptors)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.NonQueryExecuted(cmd, dbCommandInterceptionContext);
            }, globalInterceptors);
        }
        void OnScalarExecuting(IDbCommand cmd, DbCommandInterceptionContext<object> dbCommandInterceptionContext, IDbCommandInterceptor[] globalInterceptors)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.ScalarExecuting(cmd, dbCommandInterceptionContext);
            }, globalInterceptors);
        }
        void OnScalarExecuted(IDbCommand cmd, DbCommandInterceptionContext<object> dbCommandInterceptionContext, IDbCommandInterceptor[] globalInterceptors)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.ScalarExecuted(cmd, dbCommandInterceptionContext);
            }, globalInterceptors);
        }

        void ExecuteDbCommandInterceptors(Action<IDbCommandInterceptor> act, IDbCommandInterceptor[] globalInterceptors)
        {
            for (int i = 0; i < globalInterceptors.Length; i++)
            {
                act(globalInterceptors[i]);
            }

            if (this._dbCommandInterceptors != null)
            {
                for (int i = 0; i < this._dbCommandInterceptors.Count; i++)
                {
                    act(this._dbCommandInterceptors[i]);
                }
            }
        }
        #endregion


        void ReleaseTransaction()
        {
            this._dbTransaction.Dispose();
            this._dbTransaction = null;
            this._isInTransaction = false;
        }


        void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }


        public static string AppendDbCommandInfo(string cmdText, DbParam[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            if (parameters != null)
            {
                foreach (DbParam param in parameters)
                {
                    if (param == null)
                        continue;

                    string typeName = null;
                    object value = null;
                    Type parameterType;
                    if (param.Value == null || param.Value == DBNull.Value)
                    {
                        parameterType = param.Type;
                        value = "NULL";
                    }
                    else
                    {
                        value = param.Value;
                        parameterType = param.Value.GetType();

                        if (parameterType == typeof(string) || parameterType == typeof(DateTime))
                            value = "'" + value + "'";
                    }

                    if (parameterType != null)
                        typeName = GetTypeName(parameterType);

                    sb.AppendFormat("{0} {1} = {2};", typeName, param.Name, value);
                    sb.AppendLine();
                }
            }

            sb.AppendLine(cmdText);

            return sb.ToString();
        }
        static string GetTypeName(Type type)
        {
            Type underlyingType;
            if (ReflectionExtension.IsNullable(type, out underlyingType))
            {
                return string.Format("Nullable<{0}>", GetTypeName(underlyingType));
            }

            return type.Name;
        }

        static ChloeException WrapException(Exception ex)
        {
            return new ChloeException("An exception occurred while executing DbCommand. For details please see the inner exception.", ex);
        }
    }
}
