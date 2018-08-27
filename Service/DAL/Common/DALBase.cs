using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class DALBase<T> : AbstractDALBase<T> where T : ModelBase
    {
        protected DbContext db;
        protected string connName = "MySQL", areaNo;

        public DALBase()
        {
            this.db = Factory.Instance.CreateDbContext(connName);
            this.areaNo = ConfigurationManager.AppSettings["AreaNo"];
        }

        public DALBase(string connName)
        {
            this.db = Factory.Instance.CreateDbContext(connName);
            this.areaNo = ConfigurationManager.AppSettings["AreaNo"];
            this.connName = connName;
        }

        public DALBase(string connName, string areaNo)
        {
            this.db = Factory.Instance.CreateDbContext(connName);
            this.areaNo = areaNo;
            this.connName = connName;
        }

        public DALBase(DbContext db)
        {
            this.db = db;
            this.areaNo = ConfigurationManager.AppSettings["AreaNo"];
        }

        public DALBase(DbContext db, string areaNo)
        {
            this.db = db;
            this.areaNo = areaNo;
        }

        #region CommonMethods

        public override IQuery<T> GetQuery()
        {
            return db.Query<T>().Where(p => p.AreaNo == this.areaNo);
        }

        public override List<T> GetModelList()
        {
            return this.GetQuery().ToList();
        }

        public override List<T> GetModelList(Expression<Func<T, bool>> predicate)
        {
            return this.GetQuery().Where(predicate).ToList();
        }

        public override T GetModel(int id)
        {
            return db.Query<T>().Where(p => p.ID == id && p.AreaNo == this.areaNo).FirstOrDefault();
        }

        public override T GetModel(Expression<Func<T, bool>> predicate)
        {
            return this.GetQuery().Where(predicate).FirstOrDefault();
        }

        public override T Insert(T model)
        {
            model.ID = this.GetMaxId();
            model.AreaNo = this.areaNo;
            return db.Insert(model);
        }

        public override int Update(T model)
        {
            if (model.AreaNo != this.areaNo)
                return -1;
            return this.db.Update(model);
        }

        public override int Delete(T model)
        {
            if (model.AreaNo != this.areaNo)
                return -1;
            var result = this.db.Delete(model);
            this.ResetMaxId();
            return result;
        }

        public override int GetMaxId()
        {
            return this.GetMaxId(false);
        }

        public override int ResetMaxId()
        {
            return this.GetMaxId(true);
        }

        static LockDictionary lockDic = new LockDictionary();

        private int GetMaxId(bool isReset)
        {
            int maxId = -1;
            try
            {
                lock (lockDic.GetLockObject(this.tableName))
                {
                    using (var maxDb = Factory.Instance.CreateDbContext(this.connName))
                    {
                        var paraList = new DbParam[]{
                            new DbParam("areaNo",this.areaNo),
                            new DbParam("tableName",this.tableName),
                            new DbParam("maxId", null)
                        };
                        object maxObj = maxDb.Session.ExecuteScalar("select maxId+1 from f_maxid where areaNo=@areaNo and tableName=@tableName", paraList);
                        object resetObj = null;
                        if (isReset || maxObj == null || maxObj == DBNull.Value)
                        {
                            resetObj = maxDb.Session.ExecuteScalar(string.Format("select max(id) from {0} where areaNo=@areaNo", this.tableName), paraList);
                            if (resetObj == null || resetObj == DBNull.Value)
                                resetObj = 0;
                        }
                        if (maxObj == null || maxObj == DBNull.Value)
                        {
                            maxId = Convert.ToInt32(resetObj) + (isReset ? 0 : 1);
                            paraList[2].Value = maxId;
                            var insertSql = "insert into f_maxid(areaNo,tableName,maxId) values(@areaNo,@tableName,@maxId) ";
                            maxDb.Session.ExecuteNonQuery(insertSql, paraList);
                        }
                        else
                        {
                            maxId = isReset ? Convert.ToInt32(resetObj) : Convert.ToInt32(maxObj);
                            paraList[2].Value = maxId;
                            var updateSql = "update f_maxid set maxId=@maxId where areaNo=@areaNo and tableName=@tableName";
                            maxDb.Session.ExecuteNonQuery(updateSql, paraList);
                        }
                        return maxId;
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

        #endregion
    }
}
