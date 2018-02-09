using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TUnitDAL
    {
        DbContext db;
        public TUnitDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TUnitDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TUnitModel> GetModelList()
        {
            return db.Query<TUnitModel>().ToList();
        }

        public List<TUnitModel> GetModelList(Expression<Func<TUnitModel, bool>> predicate)
        {
            return db.Query<TUnitModel>().Where(predicate).ToList();
        }

        public TUnitModel GetModel(int id)
        {
            return db.Query<TUnitModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TUnitModel GetModel(Expression<Func<TUnitModel, bool>> predicate)
        {
            return db.Query<TUnitModel>().Where(predicate).FirstOrDefault();
        }

        public TUnitModel Insert(TUnitModel model)
        {
            return db.Insert(model);
        }

        public int Update(TUnitModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TUnitModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_unit AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            return db.Query<TUnitModel>().Select(s => new
            {
                s.id,
                s.unitSeq,
                s.unitName,
                s.orderNum,
                Model = s
            })
            .OrderBy(k => k.id)
            .ToList();
        }

        public TUnitModel GetModel(int areaCode, int areaId)
        {
            return db.Query<TUnitModel>().Where(p => p.areaCode == areaCode && p.areaId == areaId).FirstOrDefault();
        }
    }
}
