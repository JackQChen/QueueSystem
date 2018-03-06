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

        TAppointmentDAL dal;

        public TAppointmentBLL()
        {
            this.dal = new TAppointmentDAL();
        }

        public TAppointmentBLL(string dbKey)
        {
            this.dal = new TAppointmentDAL(dbKey);
        }

        #region CommonMethods

        public List<TAppointmentModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TAppointmentModel> GetModelList(Expression<Func<TAppointmentModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TAppointmentModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TAppointmentModel GetModel(Expression<Func<TAppointmentModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TAppointmentModel Insert(TAppointmentModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TAppointmentModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TAppointmentModel model)
        {
            return this.dal.Delete(model);
        }

        #endregion

        public bool IsBasic
        {
            get { return false; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
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
                var odal = new TAppointmentDAL(areaCode.ToString());
                foreach (var s in sList)
                {
                    dal.Insert(s);
                    s.id = s.areaId;
                    s.sysFlag = 2;
                    odal.Update(s);
                }

                return sList.Count;
            }
            catch
            {
                return -1;
            }
        }

        public int ProcessUpdateData(int areaCode, string targetDbName)
        {
            return 0;
        }

        public int ProcessDeleteData(int areaCode, string targetDbName)
        {
            return 0;
        }
    }
}
