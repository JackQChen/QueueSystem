using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TWindowAreaBLL : IGridData, IUploadData
    {

        private TWindowAreaDAL dal;

        public TWindowAreaBLL()
        {
            this.dal = new TWindowAreaDAL();
        }

        public TWindowAreaBLL(string dbKey)
        {
            this.dal = new TWindowAreaDAL(dbKey);
        }

        #region CommonMethods

        public List<TWindowAreaModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TWindowAreaModel> GetModelList(Expression<Func<TWindowAreaModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TWindowAreaModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TWindowAreaModel GetModel(Expression<Func<TWindowAreaModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TWindowAreaModel Insert(TWindowAreaModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TWindowAreaModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TWindowAreaModel model)
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
                var sList = new TWindowAreaDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TWindowAreaDAL(targetDbName);
                var odal = new TWindowAreaDAL(areaCode.ToString());
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
                var sdal = new TWindowAreaDAL(areaCode.ToString());
                var tdal = new TWindowAreaDAL(targetDbName);
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
