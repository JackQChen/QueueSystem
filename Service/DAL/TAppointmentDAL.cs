using System;
using System.Collections.Generic;
using Chloe;
using Model;

namespace DAL
{
    public class TAppointmentDAL
    {
        DbContext db;
        public TAppointmentDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        #region CommonMethods

        public List<TAppointmentModel> GetModelList()
        {
            return db.Query<TAppointmentModel>().ToList();
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
