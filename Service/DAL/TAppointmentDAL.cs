using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public TAppointmentDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
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

        public TAppointmentModel GetModel(Expression<Func<TAppointmentModel, bool>> predicate)
        {
            return db.Query<TAppointmentModel>().Where(predicate).FirstOrDefault();
        }

        public TAppointmentModel Insert(TAppointmentModel model)
        {
            return db.Insert(model);
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
