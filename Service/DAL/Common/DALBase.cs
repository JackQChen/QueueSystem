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
        protected string areaNo;

        public DALBase()
        {
            this.db = Factory.Instance.CreateDbContext();
            this.areaNo = ConfigurationManager.AppSettings["AreaNo"];
        }

        public DALBase(string connName)
        {
            this.db = Factory.Instance.CreateDbContext(connName);
            this.areaNo = ConfigurationManager.AppSettings["AreaNo"];
        }

        public DALBase(string connName, string areaNo)
        {
            this.db = Factory.Instance.CreateDbContext(connName);
            this.areaNo = areaNo;
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
            return this.db.Delete(model);
        }

        public override int GetMaxId()
        {
            var maxId = this.db.Session.ExecuteScalar(string.Format(
                //"select max(convert(int,{0}))+1 from {1}",   //sql
                "select max(cast({0} as SIGNED INTEGER))+1 from {1} where areaNo={2}",  //mysql
                "ID",
                tableName,
                this.areaNo), null).ToString();
            if (string.IsNullOrEmpty(maxId))
                return 1;
            else
                return Convert.ToInt32(maxId);
        }

        #endregion
    }
}
