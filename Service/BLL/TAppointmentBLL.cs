using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;
namespace BLL
{
    public class TAppointmentBLL : IUploadData
    {
        public TAppointmentBLL()
        {
        }

        #region CommonMethods

        public List<TAppointmentModel> GetModelList()
        {
            return new TAppointmentDAL().GetModelList();
        }

        public List<TAppointmentModel> GetModelList(Expression<Func<TAppointmentModel, bool>> predicate)
        {
            return new TAppointmentDAL().GetModelList(predicate);
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


        public bool IsBasic
        {
            get { return false; }
        }

        public bool ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TAppointmentDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TAppointmentDAL(targetDbName);
                foreach (var s in sList)
                {
                    dal.Insert(s);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessUpdateData(int areaCode, string targetDbName)
        {
            return true;
        }

        public bool ProcessDeleteData(int areaCode, string targetDbName)
        {
            return true;
        }
    }
}
