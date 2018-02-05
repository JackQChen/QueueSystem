using System.Collections.Generic;
using Chloe;
using Model;

namespace DAL
{
    public class TEvaluateDAL
    {
        DbContext db;
        public TEvaluateDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        #region CommonMethods

        public List<TEvaluateModel> GetModelList()
        {
            return db.Query<TEvaluateModel>().ToList();
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
