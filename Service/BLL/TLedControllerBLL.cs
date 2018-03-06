using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TLedControllerBLL : IGridData, IUploadData
    {

        private TLedControllerDAL dal;

        public TLedControllerBLL()
        {
            this.dal = new TLedControllerDAL();
        }

        public TLedControllerBLL(string dbKey)
        {
            this.dal = new TLedControllerDAL(dbKey);
        }

        #region CommonMethods

        public List<TLedControllerModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TLedControllerModel> GetModelList(Expression<Func<TLedControllerModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TLedControllerModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TLedControllerModel GetModel(Expression<Func<TLedControllerModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TLedControllerModel Insert(TLedControllerModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TLedControllerModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TLedControllerModel model)
        {
            return this.dal.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.dal.ResetIndex();
        }

        public object GetGridData()
        {
            return this.dal.GetGridData();
        }


        public bool IsBasic
        {
            get { return true; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TLedControllerDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.ID;
                });
                var dal = new TLedControllerDAL(targetDbName);
                var odal = new TLedControllerDAL(areaCode.ToString());
                foreach (var s in sList)
                {
                    dal.Insert(s);
                    s.ID = s.areaId;
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
            try
            {
                var sdal = new TLedControllerDAL(areaCode.ToString());
                var tdal = new TLedControllerDAL(targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.ID;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.ID).FirstOrDefault();
                    var data = s;
                    data.ID = nData.ID;
                    data.areaCode = nData.areaCode;
                    data.areaId = nData.areaId;
                    tdal.Update(data);
                    s.sysFlag = 2;
                    s.ID = id;
                    sdal.Update(s);
                }
                return sList.Count;
            }
            catch
            {
                return -1;
            }
        }

        public int ProcessDeleteData(int areaCode, string targetDbName)
        {
            return 0;
        }
    }
}
