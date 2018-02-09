using System.Collections.Generic;
using Chloe;
using Model;
using System.Linq.Expressions;
using System;

namespace DAL
{
    public class TGetCardDAL
    {
        DbContext db;
        public TGetCardDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }
        public TGetCardDAL(string dbName)
        {
            this.db = Factory.Instance.CreateDbContext(dbName);
        }

        #region CommonMethods

        public List<TGetCardModel> GetModelList()
        {
            return db.Query<TGetCardModel>().ToList();
        }

        public List<TGetCardModel> GetModelList(Expression<Func<TGetCardModel, bool>> predicate)
        {
            return db.Query<TGetCardModel>().Where(predicate).ToList();
        }

        public TGetCardModel GetModel(int id)
        {
            return db.Query<TGetCardModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TGetCardModel Insert(TGetCardModel model)
        {
            return db.Insert(model);
        }

        public int Update(TGetCardModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TGetCardModel model)
        {
            return this.db.Delete(model);
        }

        #endregion
    }
}
