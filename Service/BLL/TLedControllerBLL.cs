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
        public TLedControllerBLL()
        {
        }

        #region CommonMethods


        public List<TLedControllerModel> GetModelList()
        {
            return new TLedControllerDAL().GetModelList();
        }

        public List<TLedControllerModel> GetModelList(Expression<Func<TLedControllerModel, bool>> predicate)
        {
            return new TLedControllerDAL().GetModelList(predicate);
        }

        public TLedControllerModel GetModel(int id)
        {
            return new TLedControllerDAL().GetModel(id);
        }

        public TLedControllerModel GetModel(Expression<Func<TLedControllerModel, bool>> predicate)
        {
            return new TLedControllerDAL().GetModel(predicate);
        }

        public TLedControllerModel Insert(TLedControllerModel model)
        {
            return new TLedControllerDAL().Insert(model);
        }

        public int Update(TLedControllerModel model)
        {
            return new TLedControllerDAL().Update(model);
        }

        public int Delete(TLedControllerModel model)
        {
            return new TLedControllerDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TLedControllerDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TLedControllerDAL().GetGridData();
        }

        
        public bool IsBasic
        {
            get { return true; }
        }

        public int ProcessInsertData(int areaCode,  string targetDbName)
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

        public int ProcessUpdateData(int areaCode,  string targetDbName)
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

        public int ProcessDeleteData(int areaCode,  string targetDbName)
        {
            return 0;
        }
    }
}
