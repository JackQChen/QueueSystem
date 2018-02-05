using System;
using System.Collections.Generic;
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

        #region CommonMethods

        public List<TUnitModel> GetModelList()
        {
            return db.Query<TUnitModel>().ToList();
        }

        public TUnitModel GetModel(int id)
        {
            return db.Query<TUnitModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TUnitModel Insert(TUnitModel model)
        {
            return this.db.Insert(model);
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
    }
}
