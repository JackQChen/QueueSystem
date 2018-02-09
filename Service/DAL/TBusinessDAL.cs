using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TBusinessDAL
    {
        DbContext db;

        public TBusinessDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TBusinessDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TBusinessModel> GetModelList()
        {
            return db.Query<TBusinessModel>().ToList();
        }

        public List<TBusinessModel> GetModelList(Expression<Func<TBusinessModel, bool>> predicate)
        {
            return db.Query<TBusinessModel>().Where(predicate).ToList();
        }

        public TBusinessModel GetModel(int id)
        {
            return db.Query<TBusinessModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TBusinessModel GetModel(Expression<Func<TBusinessModel, bool>> predicate)
        {
            return db.Query<TBusinessModel>().Where(predicate).FirstOrDefault();
        }

        public TBusinessModel Insert(TBusinessModel model)
        {
            return db.Insert(model);
        }

        public int Update(TBusinessModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TBusinessModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_business AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            var dicType = new TDictionaryDAL().GetModelQuery(this.db, DictionaryString.AppointmentType);
            return db.Query<TBusinessModel>()
                .LeftJoin(dicType, (m, t) => m.busiType.ToString() == t.Value)
                .LeftJoin<TUnitModel>((m, t, u) => m.unitSeq == u.unitSeq)
                .Select((m, t, u) => new
                {
                    m.id,
                    u.unitName,
                    m.unitSeq,
                    m.busiSeq,
                    m.busiCode,
                    m.busiName,
                    busiType = t.Name,
                    acceptBusi = m.acceptBusi ? "是" : "否",
                    getBusi = m.getBusi ? "是" : "否",
                    askBusi = m.askBusi ? "是" : "否",
                    Model = m
                })
                .OrderBy(k => k.id)
                .ToList();
        }
    }
}
