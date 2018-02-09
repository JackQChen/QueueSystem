using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;
namespace BLL
{
    public class TWindowBLL : IGridData,IUploadData
    {
        public TWindowBLL()
        {
        }

        #region CommonMethods

        public List<TWindowModel> GetModelList()
        {
            return new TWindowDAL().GetModelList();
        }

        public List<TWindowModel> GetModelList(Expression<Func<TWindowModel, bool>> predicate)
        {
            return new TWindowDAL().GetModelList(predicate);
        }

        public TWindowModel GetModel(int ID)
        {
            return new TWindowDAL().GetModel(ID);
        }

        public TWindowModel Insert(TWindowModel model)
        {
            return new TWindowDAL().Insert(model);
        }

        public int Update(TWindowModel model)
        {
            return new TWindowDAL().Update(model);
        }

        public int Delete(TWindowModel model)
        {
            return new TWindowDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TWindowDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TWindowDAL().GetGridData();
        }


        public bool IsBasic
        {
            get { return true; }
        }

        public bool ProcessInsertData(int areaCode,  string targetDbName)
        {
            try
            {
                var sList = new TWindowDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.ID;
                });
                var dal = new TWindowDAL(targetDbName);
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

        public bool ProcessUpdateData(int areaCode,  string targetDbName)
        {
            try
            {
                var sdal = new TWindowDAL(areaCode.ToString());
                var tdal = new TWindowDAL(targetDbName);
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
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessDeleteData(int areaCode,  string targetDbName)
        {
            return true;
        }
    }
}
