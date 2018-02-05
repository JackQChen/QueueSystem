using System.Collections.Generic;
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

        #region CommonMethods

        public List<TGetCardModel> GetModelList()
        {
            return db.Query<TGetCardModel>().ToList();
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
