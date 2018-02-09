using System;
using System.Collections.Generic;
using Chloe;
using Model;
using System.Configuration;
using System.Linq.Expressions;

namespace DAL
{
    public class TAppointmentDAL
    {
        DbContext db;
        public TAppointmentDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TAppointmentDAL(string dbName)
        {
            this.db = Factory.Instance.CreateDbContext(dbName);
        }

        #region CommonMethods

        public List<TAppointmentModel> GetModelList()
        {
            return db.Query<TAppointmentModel>().ToList();
        }

        public List<TAppointmentModel> GetModelList(Expression<Func<TAppointmentModel, bool>> predicate)
        {
            return db.Query<TAppointmentModel>().Where(predicate).ToList();
        }

        public TAppointmentModel GetModel(int id)
        {
            return db.Query<TAppointmentModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TAppointmentModel Insert(TAppointmentModel model)
        {
            return this.db.Insert(model);
        }

        public int Update(TAppointmentModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TAppointmentModel model)
        {
            return this.db.Delete(model);
        }

        #endregion
    }
}
