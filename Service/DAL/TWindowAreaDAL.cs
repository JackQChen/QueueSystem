using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TWindowAreaDAL
    {
        DbContext db;
        public TWindowAreaDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TWindowAreaDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TWindowAreaModel> GetModelList()
        {
            return db.Query<TWindowAreaModel>().ToList();
        }

        public List<TWindowAreaModel> GetModelList(Expression<Func<TWindowAreaModel, bool>> predicate)
        {
            return db.Query<TWindowAreaModel>().Where(predicate).ToList();
        }

        public TWindowAreaModel GetModel(int id)
        {
            return db.Query<TWindowAreaModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TWindowAreaModel GetModel(Expression<Func<TWindowAreaModel, bool>> predicate)
        {
            return db.Query<TWindowAreaModel>().Where(predicate).FirstOrDefault();
        }

        public TWindowAreaModel Insert(TWindowAreaModel model)
        {
            return db.Insert(model);
        }

        public int Update(TWindowAreaModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TWindowAreaModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_windowarea AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            return db.Query<TWindowAreaModel>().Select(s => new
            {
                s.id,
                s.areaName,
                s.remark,
                Model = s
            })
            .OrderBy(k => k.id)
            .ToList();
        }

    }
}
