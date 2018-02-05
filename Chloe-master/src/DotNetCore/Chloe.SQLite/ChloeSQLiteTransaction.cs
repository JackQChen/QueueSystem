using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.SQLite
{
    class ChloeSQLiteTransaction : IDbTransaction
    {
        IDbTransaction _transaction;
        ChloeSQLiteConcurrentConnection _conn;
        bool _hasFinished = false;
        public ChloeSQLiteTransaction(IDbTransaction transaction, ChloeSQLiteConcurrentConnection conn)
        {
            this._transaction = transaction;
            this._conn = conn;

            this._conn.RWLock.BeginTransaction();
        }

        ~ChloeSQLiteTransaction()
        {
            this.Dispose();
        }

        public IDbTransaction InnerTransaction { get { return this._transaction; } }

        void EndTransaction()
        {
            if (this._hasFinished == false)
            {
                this._conn.RWLock.EndTransaction();
                this._hasFinished = true;
            }
        }


        public IDbConnection Connection { get { return this._transaction.Connection; } }
        public IsolationLevel IsolationLevel { get { return this._transaction.IsolationLevel; } }
        public void Commit()
        {
            try
            {
                this._transaction.Commit();
            }
            finally
            {
                this.EndTransaction();
            }
        }
        public void Rollback()
        {
            try
            {
                this._transaction.Rollback();
            }
            finally
            {
                this.EndTransaction();
            }
        }

        public void Dispose()
        {
            this._transaction.Dispose();
            this.EndTransaction();
            GC.SuppressFinalize(this);
        }
    }
}
