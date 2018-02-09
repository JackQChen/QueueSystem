using System;
using System.Collections.Generic;
using Chloe;
using Model;
using System.Linq.Expressions;

namespace DAL
{
    public class TDictionaryDAL
    {
        DbContext db;
        public TDictionaryDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }
        public TDictionaryDAL(string dbName)
        {
            this.db = Factory.Instance.CreateDbContext(dbName);
        }

        #region CommonMethods

        public List<TDictionaryModel> GetModelList()
        {
            return db.Query<TDictionaryModel>().ToList();
        }

        public List<TDictionaryModel> GetModelList(Expression<Func<TDictionaryModel, bool>> predicate)
        {
            return db.Query<TDictionaryModel>().Where(predicate).ToList();
        }

        public TDictionaryModel GetModel(int id)
        {
            return db.Query<TDictionaryModel>().Where(p => p.ID == id).FirstOrDefault();
        }

        public TDictionaryModel Insert(TDictionaryModel model)
        {
            return this.db.Insert(model);
        }

        public int Update(TDictionaryModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TDictionaryModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public IQuery<TDictionaryModel> GetModelQuery(string name)
        {
            return this.GetModelQuery(this.db, name);
        }

        public IQuery<TDictionaryModel> GetModelQuery(DbContext db, string name)
        {
            return db.Query<TDictionaryModel>()
                .Where(p => p.Name == name && p.Group == 0)
                .LeftJoin<TDictionaryModel>((c, i) => c.ID == i.Group)
                .Select((c, i) => i);
        }

        public List<TDictionaryModel> GetModelList(string name)
        {
            return this.GetModelQuery(name).ToList();
        }
    }
}
