using System.Collections.Generic;
using Chloe;
using Model;
using System.Linq.Expressions;
using System;

namespace DAL
{
    public class TEvaluateDAL
    {
        DbContext db;
        public TEvaluateDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }
        public TEvaluateDAL(string dbName)
        {
            this.db = Factory.Instance.CreateDbContext(dbName);
        }

        #region CommonMethods

        public List<TEvaluateModel> GetModelList()
        {
            return db.Query<TEvaluateModel>().ToList();
        }

        public List<TEvaluateModel> GetModelList(Expression<Func<TEvaluateModel, bool>> predicate)
        {
            return db.Query<TEvaluateModel>().Where(predicate).ToList();
        }

        public TEvaluateModel GetModel(int id)
        {
            return db.Query<TEvaluateModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TEvaluateModel Insert(TEvaluateModel model)
        {
            return db.Insert(model);
        }

        public int Update(TEvaluateModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TEvaluateModel model)
        {
            return this.db.Delete(model);
        }

        #endregion
    }
}
