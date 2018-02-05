using System.Collections.Generic;
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

        #region CommonMethods

        public List<TRegisterModel> GetModelList()
        {
            return db.Query<TRegisterModel>().ToList();
        }

        public TRegisterModel GetModel(int id)
        {
            return db.Query<TRegisterModel>().Where(p => p.id == id).FirstOrDefault();
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
