using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TUnitBLL : IGridData, IUploadData
    {
        public TUnitBLL()
        {
        }

        #region CommonMethods

        public List<TUnitModel> GetModelList()
        {
            return new TUnitDAL().GetModelList();
        }

        public List<TUnitModel> GetModelList(Expression<Func<TUnitModel, bool>> predicate)
        {
            return new TUnitDAL().GetModelList(predicate);
        }

        public TUnitModel GetModel(int ID)
        {
            return new TUnitDAL().GetModel(ID);
        }

        public TUnitModel Insert(TUnitModel model)
        {
            return new TUnitDAL().Insert(model);
        }

        public int Update(TUnitModel model)
        {
            return new TUnitDAL().Update(model);
        }

        public int Delete(TUnitModel model)
        {
            return new TUnitDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TUnitDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TUnitDAL().GetGridData();
        }

        public bool IsBasic
        {
            get
            {
                return true;
            }
        }

        public bool ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TUnitDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TUnitDAL(targetDbName);
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
            try
            {
                var sdal = new TUnitDAL(areaCode.ToString());
                var tdal = new TUnitDAL(targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.id;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.id).FirstOrDefault();
                    var data = s;
                    data.id = nData.id;
                    data.areaCode = nData.areaCode;
                    data.areaId = nData.areaId;
                    tdal.Update(data);
                    s.sysFlag = 2;
                    s.id = id;
                    sdal.Update(s);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessDeleteData(int areaCode, string targetDbName)
        {
            return true;
        }
    }
}
