using System.Collections.Generic;
using Chloe;
using Model;
using System.Linq.Expressions;
using System;

namespace DAL
{
    public class TLedControllerDAL
    {
        DbContext db;
        public TLedControllerDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }
        public TLedControllerDAL(string dbName)
        {
            this.db = Factory.Instance.CreateDbContext(dbName);
        }

        #region CommonMethods

        public List<TLedControllerModel> GetModelList()
        {
            return db.Query<TLedControllerModel>().ToList();
        }

        public List<TLedControllerModel> GetModelList(Expression<Func<TLedControllerModel, bool>> predicate)
        {
            return db.Query<TLedControllerModel>().Where(predicate).ToList();
        }

        public TLedControllerModel GetModel(int id)
        {
            return db.Query<TLedControllerModel>().Where(p => p.ID == id).FirstOrDefault();
        }

        public TLedControllerModel Insert(TLedControllerModel model)
        {
            return db.Insert(model);
        }

        public int Update(TLedControllerModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TLedControllerModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_ledcontroller AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            return db.Query<TLedControllerModel>().Select(s => new
            {
                s.ID,
                s.IP,
                s.Port,
                s.DeviceAddress,
                s.Name,
                Model = s
            })
            .OrderBy(k => k.ID)
            .ToList();
        }
    }
}
