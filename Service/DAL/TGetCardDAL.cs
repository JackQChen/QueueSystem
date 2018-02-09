using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TGetCardDAL
    {
        DbContext db;

        public TGetCardDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TGetCardDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
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

        public TGetCardModel GetModel(Expression<Func<TGetCardModel, bool>> predicate)
        {
            return db.Query<TGetCardModel>().Where(predicate).FirstOrDefault();
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
