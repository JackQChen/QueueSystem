using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TLedWindowBLL : IGridData, IUploadData
    {

        private TLedWindowDAL dal;

        public TLedWindowBLL()
        {
            this.dal = new TLedWindowDAL();
        }

        public TLedWindowBLL(string dbKey)
        {
            this.dal = new TLedWindowDAL(dbKey);
        }

        #region CommonMethods

        public List<TLedWindowModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TLedWindowModel> GetModelList(Expression<Func<TLedWindowModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TLedWindowModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TLedWindowModel GetModel(Expression<Func<TLedWindowModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TLedWindowModel Insert(TLedWindowModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TLedWindowModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TLedWindowModel model)
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
            return new TLedControllerDAL().GetGridData();
        }

        public object GetGridDataByControllerId(int controllerId)
        {
            return this.dal.GetGridDataByControllerId(controllerId);
        }


        public bool IsBasic
        {
            get { return true; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TLedWindowDAL(areaCode.ToString()).GetModelList().Where(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.ID;
                });
                var dal = new TLedWindowDAL(targetDbName);
                var odal = new TLedWindowDAL(areaCode.ToString());
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
                var sdal = new TLedWindowDAL(areaCode.ToString());
                var tdal = new TLedWindowDAL(targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.ID;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.ID).FirstOrDefault();
                    if (nData == null)
                    {
                        s.areaCode = areaCode;
                        s.areaId = s.ID;
                        tdal.Insert(s);
                        s.ID = s.areaId;
                        s.sysFlag = 2;
                        sdal.Update(s);
                    }
                    else
                    {
                        var data = s;
                        data.ID = nData.ID;
                        data.areaCode = nData.areaCode;
                        data.areaId = nData.areaId;
                        tdal.Update(data);
                        s.sysFlag = 2;
                        s.ID = id;
                        sdal.Update(s);
                    }
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
