using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chloe;
using Model;

namespace DAL
{

    public class LockDAL : DALBase<FLockModel>
    {
        static LockDictionary lockDic = new LockDictionary();
        public LockDAL()
            : base()
        {
        }

        public LockDAL(DbContext db)
            : base(db)
        {
        }

        public LockDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public LockDAL(string connName)
            : base(connName)
        {
        }

        public LockDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public bool lockWin(string windowNo)
        {
            try
            {
                lock (lockDic.GetLockObject(windowNo))
                {
                    var model = this.GetQuery().Where(p => p.windowNo == windowNo).FirstOrDefault();
                    if (model == null)
                    {
                        model = new FLockModel();
                        model.windowNo = windowNo;
                        model.isOccupy = 1;
                        this.Insert(model);
                        return true;
                    }
                    else
                    {
                        if (model.isOccupy == 0)
                        {
                            model.isOccupy = 1;
                            this.Update(model);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw ex;
            }
        }

        public void releaseWin(string windowNo)
        {
            lock (lockDic.GetLockObject(windowNo))
            {
                var model = this.GetQuery().Where(p => p.windowNo == windowNo).FirstOrDefault();
                model.isOccupy = 0;
                this.Update(model);
            }
        }
    }
}
