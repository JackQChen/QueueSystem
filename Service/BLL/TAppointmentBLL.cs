using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TAppointmentBLL
    {
        public TAppointmentBLL()
        {
        }

        #region CommonMethods

        public List<TAppointmentModel> GetModelList()
        {
            return new TAppointmentDAL().GetModelList();
        }

        public TAppointmentModel GetModel(int id)
        {
            return new TAppointmentDAL().GetModel(id);
        }

        public TAppointmentModel Insert(TAppointmentModel model)
        {
            return new TAppointmentDAL().Insert(model);
        }

        public int Update(TAppointmentModel model)
        {
            return new TAppointmentDAL().Update(model);
        }

        public int Delete(TAppointmentModel model)
        {
            return new TAppointmentDAL().Delete(model);
        }

        #endregion
    }
}
