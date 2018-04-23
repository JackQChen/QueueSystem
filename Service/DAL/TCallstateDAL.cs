using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TCallStateDAL
    {

        private DbContext db;

        public TCallStateDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TCallStateDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TCallStateModel> GetModelList()
        {
            return db.Query<TCallStateModel>().ToList();
        }

        public List<TCallStateModel> GetModelList(Expression<Func<TCallStateModel, bool>> predicate)
        {
            return db.Query<TCallStateModel>().Where(predicate).ToList();
        }

        public TCallStateModel GetModel(string id)
        {
            return db.Query<TCallStateModel>().Where(p => p.windowNo == id).FirstOrDefault();
        }

        public TCallStateModel GetModel(Expression<Func<TCallStateModel, bool>> predicate)
        {
            return db.Query<TCallStateModel>().Where(predicate).FirstOrDefault();
        }

        public TCallStateModel Insert(TCallStateModel model)
        {
            return db.Insert(model);
        }

        public int Update(TCallStateModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TCallStateModel model)
        {
            return this.db.Delete(model);
        }

        #endregion
    }
}
