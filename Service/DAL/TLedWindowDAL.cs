using System.Collections.Generic;
using Chloe;
using Model;
using System.Linq.Expressions;
using System;

namespace DAL
{
    public class TLedWindowDAL
    {
        DbContext db;
        public TLedWindowDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }
        public TLedWindowDAL(string dbName)
        {
            this.db = Factory.Instance.CreateDbContext(dbName);
        }

        #region CommonMethods

        public List<TLedWindowModel> GetModelList()
        {
            return db.Query<TLedWindowModel>().ToList();
        }

        public List<TLedWindowModel> GetModelList(Expression<Func<TLedWindowModel, bool>> predicate)
        {
            return db.Query<TLedWindowModel>().Where(predicate).ToList();
        }

        public TLedWindowModel GetModel(int id)
        {
            return db.Query<TLedWindowModel>().Where(p => p.ID == id).FirstOrDefault();
        }

        public TLedWindowModel Insert(TLedWindowModel model)
        {
            return db.Insert(model);
        }

        public int Update(TLedWindowModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TLedWindowModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_ledwindow AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridDataByControllerId(int controllerId)
        {
            return db.Query<TLedWindowModel>().Where(m => m.ControllerID == controllerId)
            .LeftJoin<TWindowModel>((m, w) => m.WindowNumber == w.Number)
            .Select((m, w) => new
            {
                m.ID,
                WindowNumber = w.Number,
                WindowName = w.Name,
                m.DisplayText,
                m.Position,
                Model = m
            })
            .OrderBy(k => k.WindowNumber)
            .ToList();
        }

    }
}
