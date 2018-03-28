using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;
using System.Collections;

namespace BLL
{
    public class TUnitBLL : IGridData, IUploadData
    {

        private TUnitDAL dal;

        public TUnitBLL()
        {
            this.dal = new TUnitDAL();
        }

        public TUnitBLL(string dbKey)
        {
            this.dal = new TUnitDAL(dbKey);
        }

        #region CommonMethods

        public List<TUnitModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TUnitModel> GetModelList(Expression<Func<TUnitModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TUnitModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TUnitModel GetModel(Expression<Func<TUnitModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TUnitModel Insert(TUnitModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TUnitModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TUnitModel model)
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
            get
            {
                return true;
            }
        }

        public ArrayList UploadUnitAndBusy(List<TUnitModel> uList, List<TBusinessModel> bList)
        {
            return this.dal.UploadUnitAndBusy(uList, bList);
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
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
                var odal = new TUnitDAL(areaCode.ToString());
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
            try
            {
                var sdal = new TUnitDAL(areaCode.ToString());
                var tdal = new TUnitDAL(targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.id;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.id).FirstOrDefault();
                    if (nData == null)
                    {
                        s.areaCode = areaCode;
                        s.areaId = s.id;
                        tdal.Insert(s);
                        s.id = s.areaId;
                        s.sysFlag = 2;
                        sdal.Update(s);
                    }
                    else
                    {
                        var data = s;
                        data.id = nData.id;
                        data.areaCode = nData.areaCode;
                        data.areaId = nData.areaId;
                        tdal.Update(data);
                        s.sysFlag = 2;
                        s.id = id;
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
