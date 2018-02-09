using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TRegisterDAL
    {
        DbContext db;
        public TRegisterDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TRegisterDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TRegisterModel> GetModelList()
        {
            return db.Query<TRegisterModel>().ToList();
        }

        public List<TRegisterModel> GetModelList(Expression<Func<TRegisterModel, bool>> predicate)
        {
            return db.Query<TRegisterModel>().Where(predicate).ToList();
        }

        public TRegisterModel GetModel(int id)
        {
            return db.Query<TRegisterModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TRegisterModel GetModel(Expression<Func<TRegisterModel, bool>> predicate)
        {
            return db.Query<TRegisterModel>().Where(predicate).FirstOrDefault();
        }

        public TRegisterModel Insert(TRegisterModel model)
        {
            return db.Insert(model);
        }

        public int Update(TRegisterModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TRegisterModel model)
        {
            return this.db.Delete(model);
        }

        #endregion
    }
}
