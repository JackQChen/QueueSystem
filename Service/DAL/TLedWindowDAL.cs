using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TLedWindowDAL
    {
        DbContext db;
        public TLedWindowDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TLedWindowDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
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

        public TLedWindowModel GetModel(Expression<Func<TLedWindowModel, bool>> predicate)
        {
            return db.Query<TLedWindowModel>().Where(predicate).FirstOrDefault();
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
