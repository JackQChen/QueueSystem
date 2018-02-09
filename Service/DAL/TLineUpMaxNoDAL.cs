using System.Collections.Generic;
using Chloe;
using Model;
using System.Linq.Expressions;
using System;

namespace DAL
{
    public class TLineUpMaxNoDAL
    {
        DbContext db;
        public TLineUpMaxNoDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TLineUpMaxNoDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        public TLineUpMaxNoDAL(DbContext db)
        {
            this.db = db;
        }

        #region CommonMethods

        public List<TLineUpMaxNoModel> GetModelList()
        {
            return db.Query<TLineUpMaxNoModel>().ToList();
        }

        public List<TLineUpMaxNoModel> GetModelList(Expression<Func<TLineUpMaxNoModel, bool>> predicate)
        {
            return db.Query<TLineUpMaxNoModel>().Where(predicate).ToList();
        }

        public TLineUpMaxNoModel GetModel(int id)
        {
            return db.Query<TLineUpMaxNoModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TLineUpMaxNoModel Insert(TLineUpMaxNoModel model)
        {
            return db.Insert(model);
        }

        public int Update(TLineUpMaxNoModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TLineUpMaxNoModel model)
        {
            return this.db.Delete(model);
        }

        #endregion
    }
}
