using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chloe;
using Model;

namespace DAL
{

    public class LockAction
    {

        public static void Run(FLockKey key, Action action)
        {
            using (var db = Factory.Instance.CreateDbContext("MySQL"))
            {
                try
                {
                    db.Session.CommandTimeout = 60;
                    db.Session.BeginTransaction();
                    db.Session.ExecuteScalar(@"
                    select `key` from f_lock where `key` = @key FOR UPDATE;",
                     new DbParam[] { new DbParam("key", key.ToString()) });
                    action();
                    db.Session.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.Session.RollbackTransaction();
                    throw ex;
                }
            }
        }

        public static void RunWindowLock(string windowNo, Action action)
        {
            using (var db = Factory.Instance.CreateDbContext("MySQL"))
            {
                try
                {
                    db.Session.CommandTimeout = 60;
                    db.Session.BeginTransaction();
                    db.Session.ExecuteScalar(@"
                    select `windowNo` from f_windowlock where `windowNo` = @key FOR UPDATE;",
                     new DbParam[] { new DbParam("key", windowNo.ToString()) });
                    action();
                    db.Session.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.Session.RollbackTransaction();
                    throw ex;
                }
            }
        }
    }
}
