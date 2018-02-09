using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TBusinessAttributeDAL
    {
        DbContext db;
        public TBusinessAttributeDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TBusinessAttributeDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TBusinessAttributeModel> GetModelList()
        {
            return db.Query<TBusinessAttributeModel>().ToList();
        }

        public List<TBusinessAttributeModel> GetModelList(Expression<Func<TBusinessAttributeModel, bool>> predicate)
        {
            return db.Query<TBusinessAttributeModel>().Where(predicate).ToList();
        }

        public TBusinessAttributeModel GetModel(int id)
        {
            return db.Query<TBusinessAttributeModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TBusinessAttributeModel GetModel(Expression<Func<TBusinessAttributeModel, bool>> predicate)
        {
            return db.Query<TBusinessAttributeModel>().Where(predicate).FirstOrDefault();
        }

        public TBusinessAttributeModel Insert(TBusinessAttributeModel model)
        {
            return db.Insert(model);
        }

        public int Update(TBusinessAttributeModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TBusinessAttributeModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_businessattribute AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            return db.Query<TBusinessModel>()
                .GroupBy(k => k.unitSeq)
                .Select(s => new { s.unitSeq })
                .LeftJoin<TUnitModel>((b, u) => b.unitSeq == u.unitSeq)
                .Select((b, u) => u)
            .OrderBy(k => k.unitSeq)
            .ToList();
        }

        public object GetGridDataByUnitSeq(string unitSeq)
        {
            var dicType = new TDictionaryDAL().GetModelQuery(this.db, DictionaryString.AppointmentType);
            return db.Query<TBusinessModel>()
                .LeftJoin(dicType, (m, t) => m.busiType.ToString() == t.Value)
                .Where((m, t) => m.unitSeq == unitSeq)
                .Select((m, t) => new
                {
                    m.unitSeq,
                    m.busiSeq,
                    m.busiCode,
                    m.busiName,
                    busiType = t.Name,
                    acceptBusi = m.acceptBusi ? "是" : "否",
                    getBusi = m.getBusi ? "是" : "否",
                    askBusi = m.askBusi ? "是" : "否"
                })
                .OrderBy(k => k.unitSeq)
                .ToList();
        }

        public object GetGridDetailData(string unitSeq, string busiSeq)
        {
            return db.JoinQuery<TBusinessAttributeModel, TUnitModel, TBusinessModel>((m, u, b) => new object[] {
                JoinType.LeftJoin, m.unitSeq == u.unitSeq  ,
                JoinType.LeftJoin,m.busiSeq == b.busiSeq && m.unitSeq == b.unitSeq
            })
            .Where((m, u, b) => m.unitSeq == unitSeq && m.busiSeq == busiSeq)
          .Select((m, u, b) => new
          {
              m.id,
              u.unitSeq,
              u.unitName,
              b.busiSeq,
              b.busiCode,
              b.busiName,
              m.timeInterval,
              m.ticketRestriction,
              m.lineUpMax,
              m.lineUpWarningMax,
              m.ticketPrefix,
              m.remark,
              Model = m
          })
          .OrderBy(k => k.id)
          .ToList();
        }
    }
}
