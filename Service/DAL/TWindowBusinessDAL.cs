using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TWindowBusinessDAL
    {
        DbContext db;
        public TWindowBusinessDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TWindowBusinessDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TWindowBusinessModel> GetModelList()
        {
            return db.Query<TWindowBusinessModel>().ToList();
        }

        public List<TWindowBusinessModel> GetModelList(Expression<Func<TWindowBusinessModel, bool>> predicate)
        {
            return db.Query<TWindowBusinessModel>().Where(predicate).ToList();
        }

        public TWindowBusinessModel GetModel(int id)
        {
            return db.Query<TWindowBusinessModel>().Where(p => p.ID == id).FirstOrDefault();
        }

        public TWindowBusinessModel GetModel(Expression<Func<TWindowBusinessModel, bool>> predicate)
        {
            return db.Query<TWindowBusinessModel>().Where(predicate).FirstOrDefault();
        }

        public TWindowBusinessModel Insert(TWindowBusinessModel model)
        {
            return db.Insert(model);
        }

        public int Update(TWindowBusinessModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TWindowBusinessModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_windowbusiness AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridBusiData(int winId)
        {
            return this.db.Query<TWindowBusinessModel>()
                .Where(m => m.WindowID == winId)
                .InnerJoin<TUnitModel>((m, u) => m.unitSeq == u.unitSeq)
                .InnerJoin<TBusinessModel>((m, u, b) => m.busiSeq == b.busiSeq && m.unitSeq == b.unitSeq)
                .Select((m, u, b) => new
                {
                    m.ID,
                    m.WindowID,
                    m.unitSeq,
                    u.unitName,
                    m.busiSeq,
                    b.busiName
                })
                .OrderBy(k => k.unitSeq)
                .ToList();
        }

        public object GetGridUserData(int winId)
        {
            return this.db.Query<TWindowBusinessModel>()
                .Where(m => m.WindowID == winId)
                .GroupBy(k => k.unitSeq)
                .Select(s => new { s.ID, s.unitSeq })
                .InnerJoin<TUserModel>((m, u) => m.unitSeq == u.unitSeq)
                .Select((m, u) => new
                {
                    ID = m.ID,
                    UserID = u.ID,
                    UserName = u.Name
                })
                .OrderBy(k => k.UserID)
                .ToList();
        }
    }
}
