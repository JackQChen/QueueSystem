using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.SQLite
{
    class ChloeSQLiteCommand : IDbCommand, IDisposable
    {
        IDbCommand _dbCommand;
        ChloeSQLiteConcurrentConnection _conn;
        public ChloeSQLiteCommand(IDbCommand dbCommand, ChloeSQLiteConcurrentConnection conn)
        {
            this._dbCommand = dbCommand;
            this._conn = conn;
        }

        public ChloeSQLiteConcurrentConnection Conn { get { return this._conn; } }

        public string CommandText
        {
            get
            {
                return this._dbCommand.CommandText;
            }
            set
            {
                this._dbCommand.CommandText = value;
            }
        }
        public int CommandTimeout
        {
            get
            {
                return this._dbCommand.CommandTimeout;
            }
            set
            {
                this._dbCommand.CommandTimeout = value;
            }
        }
        public CommandType CommandType
        {
            get
            {
                return this._dbCommand.CommandType;
            }
            set
            {
                this._dbCommand.CommandType = value;
            }
        }
        public IDbConnection Connection
        {
            get
            {
                return this._dbCommand.Connection;
            }
            set
            {
                this._dbCommand.Connection = value;
            }
        }
        public IDataParameterCollection Parameters
        {
            get
            {
                return this._dbCommand.Parameters;
            }
        }
        public IDbTransaction Transaction
        {
            get
            {
                return this._dbCommand.Transaction;
            }
            set
            {
                ChloeSQLiteTransaction tran = value as ChloeSQLiteTransaction;
                if (tran != null)
                    this._dbCommand.Transaction = tran.InnerTransaction;
                else
                    this._dbCommand.Transaction = value;
            }
        }
        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this._dbCommand.UpdatedRowSource;
            }
            set
            {
                this._dbCommand.UpdatedRowSource = value;
            }
        }

        public void Cancel()
        {
            this._dbCommand.Cancel();
        }
        public IDbDataParameter CreateParameter()
        {
            return this._dbCommand.CreateParameter();
        }
        public int ExecuteNonQuery()
        {
            this._conn.RWLock.BeginWrite();
            try
            {
                return this._dbCommand.ExecuteNonQuery();
            }
            finally
            {
                this._conn.RWLock.EndWrite();
            }
        }
        public IDataReader ExecuteReader()
        {
            this._conn.RWLock.BeginRead();
            try
            {
                return new ChloeSQLiteDataReader(this._dbCommand.ExecuteReader(), this);
            }
            catch
            {
                this._conn.RWLock.EndRead();
                throw;
            }
        }
        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            this._conn.RWLock.BeginRead();
            try
            {
                /* 不出异常的话要锁的释放留给 ChloeSQLiteDataReader 去执行 */
                return new ChloeSQLiteDataReader(this._dbCommand.ExecuteReader(behavior), this);
            }
            catch
            {
                /* 出异常的话要释放锁 */
                this._conn.RWLock.EndRead();
                throw;
            }
        }
        public object ExecuteScalar()
        {
            this._conn.RWLock.BeginRead();
            try
            {
                return this._dbCommand.ExecuteScalar();
            }
            finally
            {
                this._conn.RWLock.EndRead();
            }
        }
        public void Prepare()
        {
            this._dbCommand.Prepare();
        }
        public void Dispose()
        {
            this._dbCommand.Dispose();
        }
    }
}
