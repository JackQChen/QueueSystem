using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.MySql
{
    public class ChloeMySqlCommand : IDbCommand, IDisposable
    {
        IDbCommand _dbCommand;
        public ChloeMySqlCommand(IDbCommand dbCommand)
        {
            Utils.CheckNull(dbCommand);
            this._dbCommand = dbCommand;
        }

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
            return this._dbCommand.ExecuteNonQuery();
        }
        public IDataReader ExecuteReader()
        {
            return new ChloeMySqlDataReader(this._dbCommand.ExecuteReader());
        }
        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return new ChloeMySqlDataReader(this._dbCommand.ExecuteReader(behavior));
        }
        public object ExecuteScalar()
        {
            return this._dbCommand.ExecuteScalar();
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
