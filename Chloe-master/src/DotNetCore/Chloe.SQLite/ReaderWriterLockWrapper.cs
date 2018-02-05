using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Chloe.SQLite
{
    class ReaderWriterLockWrapper
    {
        ReaderWriterLockSlim _rwLock;
        bool _isInTransaction = false;
        public ReaderWriterLockWrapper(ReaderWriterLockSlim rwLock)
        {
            this._rwLock = rwLock;
        }

        public bool IsInTransaction { get { return this._isInTransaction; } }
        public bool IsLockHeld { get { return this._rwLock.IsReadLockHeld || this._rwLock.IsWriteLockHeld || this._rwLock.IsUpgradeableReadLockHeld; } }
        public void BeginTransaction()
        {
            if (this._isInTransaction)
                throw new Exception("Cannot call this method repeated.");

            this._rwLock.EnterWriteLock();
            this._isInTransaction = true;
        }
        public void EndTransaction()
        {
            if (this._isInTransaction)
            {
                if (this._rwLock.IsWriteLockHeld)
                    this._rwLock.ExitWriteLock();
                this._isInTransaction = false;
            }
        }

        public void BeginRead()
        {
            if (!this._isInTransaction)
                this._rwLock.EnterReadLock();
        }
        public void EndRead()
        {
            if (!this._isInTransaction)
                if (this._rwLock.IsReadLockHeld)
                    this._rwLock.ExitReadLock();
        }

        public void BeginWrite()
        {
            if (!this._isInTransaction)
                this._rwLock.EnterWriteLock();
        }
        public void EndWrite()
        {
            if (!this._isInTransaction)
                if (this._rwLock.IsWriteLockHeld)
                    this._rwLock.ExitWriteLock();
        }
    }
}
